# AGENTS.md

Цей документ містить опис архітектурних патернів, структури проєкту та правил найменування для автоматизованих агентів (AI Agents) та розробників, які працюють із цим кодовим базисом.

---

## 1. Архітектурний огляд (Architecture Overview)

Проєкт побудовано за принципами **Модульного Моноліту (Modular Monolith)** та **Чистої Архітектури (Clean Architecture)** на платформі **.NET 10**.

### Ключові принципи:
* **Модульність**: Кожна бізнес-домена розміщена в окремому модулі у директорії `src/Modules/` (наприклад, `Courses`, `Identity`, `Learning`).
* **Ізоляція шарів**: Кожен модуль розділений на 5 проєктів:
  1. `Domain` — доменні сутності, бізнес-правила, базові моделі. Не має зовнішніх залежностей.
  2. `Application` — CQRS (команди, запити), DTO, pipeline behaviors, інтерфейси репозиторіїв та Unit of Work, FluentValidation.
  3. `Persistence` — EF Core DbContext, конфігурації сутностей, реалізація Unit of Work та Repository, міграції.
  4. `Presentation` — контролери, представлення (Views) або API-ендпоінти конкретного модуля.
  5. `Integration` — відкриті контракти, DTO подій та інтерфейси для міжмодульної взаємодії.
* **Хост-додаток (Web Host)**: Проєкт `src/API/LMS` (ASP.NET Core MVC) слугує точкою входу, підключає модулі та налаштовує HTTP-пайплайн з підтримкою Razor Views.

---

## 2. Структура папок (Directory Structure)

```text
LMS_MVC/
├── LMS_MVC.slnx                                  # Solution-файл проєкту
├── AGENTS.md                                     # Правила та контекст для агентів
├── src/
│   ├── API/
│   │   ├── LMS/                                  # Головний Web Host (ASP.NET Core MVC)
│   │   │   ├── Controllers/
│   │   │   ├── Views/
│   │   │   └── Program.cs                        # Точка реєстрації DI та middleware
│   │   └── Shared/                               # Спільні компоненти для API
│   └── Modules/
│       ├── Courses/                              # Модуль "Курси"
│       │   ├── Courses.Domain/                   # Entities, BaseEntity
│       │   ├── Courses.Application/              # CQRS (Commands/Queries), DTOs, Interfaces, Behaviors
│       │   ├── Courses.Persistence/              # DbContext, EF Configurations, Repositories, Migrations
│       │   ├── Courses.Presentation/             # Controllers/Views модуля (MVC)
│       │   └── Courses.Integration/              # Інтеграційні контракти та події між модулями
│       ├── Identity/                             # Модуль "Автентифікація та Користувачі"
│       └── Learning/                             # Модуль "Навчальний процес"
```

---

## 3. Реалізовані патерни (Design Patterns & Practices)

### 3.1. CQRS та MediatR (Vertical Slice inside Application)
Кожна операція (Command або Query) реалізується за допомогою патерну **Vertical Slice / Feature Packaging** у вигляді єдиного статичного класу-контейнера.

**Структура файлу команди/запиту:**
```csharp
namespace Courses.Application.Course.Commands;

public static class CreateCourse
{
    // 1. DTO / Command Definition
    public sealed record Command(CreateCourseDto Dto) : ICommand<Result<CourseResponseDto>>;

    // 2. FluentValidation Validator
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Title).NotEmpty().WithMessage("Title cannot be empty.");
        }
    }

    // 3. MediatR Request Handler
    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<CourseResponseDto>>
    {
        public async Task<Result<CourseResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Логіка обробки...
            return Result<CourseResponseDto>.Success(/* ... */);
        }
    }
}
```

### 3.2. MediatR Pipeline Behaviors
У шарі Application реалізовано автоматичні пайплайни для обробки запитів (виконуються у строгому порядку):
1. **`LoggingBehavior<TRequest, TResponse>`**:
   - Логує початок, час виконання та помилки запиту.
2. **`ValidationBehavior<TRequest, TResponse>`**:
   - Автоматично знаходить та запускає всі `IValidator<TRequest>` перед виконанням хендлера.
   - У разі виявлення помилок повертає помилку валідації або викидає `ValidationException`.
3. **`TransactionBehavior<TRequest, TResponse>`**:
   - Відстежує запити, що реалізують інтерфейс `IBaseCommand` (`ICommand` / `ICommand<TResponse>`).
   - Автоматично загортає виконання команди в БД-транзакцію через `IExecutionStrategy` з підтримкою `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync` та логуванням.

### 3.3. Unit of Work & Generic Repository
* **`IRepository<TEntity>`**: Універсальний інтерфейс репозиторію (`GetByIdAsync`, `FirstOrDefaultAsync`, `FindAsync`, `AnyAsync`, `Query`, `AddAsync`, `Update`, `Remove`).
* **`IUnitOfWork`**: Надає доступ до репозиторіїв сутностей модуля (`Courses`, `Lessons`, `Tests`, `Reviews`), керує збереженням змін (`SaveChangesAsync`) та транзакціями.
* **Реалізація**: `UnitOfWork` використовує ліниву ініціалізацію (lazy initialization) репозиторіїв та працює з відповідним `DbContext` модуля.

### 3.4. Result Pattern
Для повернення результатів виконання команд та запитів замість викидання виключень використовується уніфікована обгортка `Result<T>`:
* `Result<T>.Success(T value)` — успішне виконання.
* `Result<T>.Failure(string error)` — помилка виконання з описом причини.

### 3.5. Encapsulated Domain Model (DDD-Lite)
* Доменні сутності успадковуються від `BaseEntity` (`Guid Id`).
* Властивості сутностей мають модифікатор **`private set`**.
* Зміна стану сутностей здійснюється виключно через доменні методи.
* Колекції інкапсульовані: внутрішнє поле `List<T> _items` експонується назовні як `IReadOnlyCollection<T>`.

---

## 4. Правила найменування та кодингу (Naming Conventions)

### 4.1. Проєкти та Простори Назв (Namespaces)
* **Проєкти**: `[ModuleName].[Layer]` (наприклад, `Courses.Domain`, `Courses.Application`, `Courses.Persistence`, `Courses.Presentation`, `Courses.Integration`).
* **Namespaces**: Повністю відповідають шляху до папки відносно проєкту.

### 4.2. Класи та Файли
* **Фічі CQRS**: Статичний клас-контейнер `[Verb][Entity]` (наприклад, `CreateCourse`). Вкладені типи: `Command`/`Query`, `Validator`, `Handler`.
* **DTO**: `Create[Entity]Dto`, `Update[Entity]Dto`, `[Entity]ResponseDto`.
* **Сутності**: В однині (`Course`, `Lesson`).
* **Конфігурації EF Core**: `[Entity]Configuration.cs`.

### 4.3. C# та Синтаксис
* Використовувати можливості **C# 12 / .NET 10**: Primary Constructors, File-scoped namespaces, Record-типи для команд та DTO.
* Приватні поля класичного типу позначаються префіксом: `_context`, `_lessons`.

### 4.4. Контролери (MVC Presentation Layer)
Оскільки хост-додаток та UI модулів є **ASP.NET Core MVC** проєктом (а не просто REST API), контролери мають відповідну специфіку:
* Контролери успадковують стандартний `Controller` (з поверненням `IActionResult`).
* Замість JSON-відповідей за замовчуванням використовуються Razor Views (`return View()`, `RedirectToAction()`).
* Для передачі даних у View використовуються `Result.Value` (ViewModels / DTOs).
* При невдалому виконанні команди (`!result.IsSuccess`) помилка додається до `ModelState`, а користувач повертається на ту саму форму.
* `IMediator` ін'єктується через Primary Constructor або базовий контролер.

**Приклад MVC Контролера:**
```csharp
[Route("courses")]
public class CoursesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCourses.Query(), ct);
        return View(result.Value); // Повертає список курсів у View
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCourseDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto); // Повернення форми з помилками валідації DTO

        var result = await mediator.Send(new CreateCourse.Command(dto), ct);
        
        if (result.IsSuccess)
        {
            // Успішне створення — редірект на список
            return RedirectToAction(nameof(Index));
        }

        // Бізнес-помилка — додаємо в ModelState і повертаємо форму
        ModelState.AddModelError(string.Empty, result.Error);
        return View(dto);
    }
}
```

---

## 5. Інструкції для AI Агентів при виконанні завдань

1. **Додавання нової фічі (Command/Query)**:
   * Створювати статичний клас у `src/Modules/[ModuleName]/[ModuleName].Application/[DomainGroup]/Commands/` або `/Queries/`.
   * Завжди використовувати типізований результат: `ICommand<Result<TDto>>` або `IQuery<Result<TDto>>`.
2. **Робота з базою даних**:
   * Зміни в схемі описувати у `[ModuleName].Domain` та `[ModuleName].Persistence/Configurations/`.
   * Завжди додавати EF Core Fluent API конфігурації замість Data Annotations.
   * Генерувати та реєструвати міграції суто у проєкті `Persistence`.
3. **Презентаційний шар (MVC)**:
   * При створенні UI завжди враховувати, що це MVC. Не генерувати API-ендпоінти (`[ApiController]`), якщо не було прямої вказівки створити REST API. Працювати з формами, Razor Views та `ModelState`.
4. **Міжмодульна взаємодія**:
   * Використовувати виключно контракти з `[ModuleName].Integration`.
   * Прямі залежності між `Application`, `Domain` або `Persistence` різних модулів **суворо заборонені**.
5. **Дотримання чистоти шарів**:
   * `Domain` не посилається на зовнішні бібліотеки.
   * `Application` посилається лише на `Domain` (і `Integration`).
   * `Persistence` посилається на `Domain` та `Application`.

# Database Schema & EF Core Rules

Цей проєкт — Модульний Моноліт. Кожен модуль має власну ізольовану схему в БД.
**КРИТИЧНЕ ПРАВИЛО:** Прямі навігаційні властивості (Navigation Properties) та Foreign Keys (FK) між різними схемами/модулями СУВОРО ЗАБОРОНЕНІ.
Замість них використовуються "Soft FK" — звичайні властивості типу `Guid`. EF Core не повинен знати про зв'язок між модулями.

## Схема БД (DBML)

```dbml
// --- SCHEMA: identity ---
Table identity.Users {
  Id uniqueidentifier [pk]
  Email varchar
  PasswordHash varchar
  Role varchar // Student, Instructor, Admin
}

// --- SCHEMA: courses ---
Table courses.Courses {
  Id uniqueidentifier [pk]
  InstructorId uniqueidentifier [note: 'Soft FK to identity.Users']
  Title varchar
  Description text
  IsPublished boolean
}

Table courses.Lessons {
  Id uniqueidentifier [pk]
  CourseId uniqueidentifier [ref: > courses.Courses.Id]
  Title varchar
  VideoUrl varchar
  OrderIndex int
}

Table courses.Tests {
  Id uniqueidentifier [pk]
  LessonId uniqueidentifier [ref: - courses.Lessons.Id] // 1 до 1 або 1 до багатьох
  Title varchar
  MaxScore int
}

Table courses.Reviews {
  Id uniqueidentifier [pk]
  CourseId uniqueidentifier [ref: > courses.Courses.Id]
  StudentId uniqueidentifier [note: 'Soft FK to identity.Users']
  Rating int
  Comment text
  IsApproved boolean
}

// --- SCHEMA: learning ---
Table learning.Enrollments {
  Id uniqueidentifier [pk]
  StudentId uniqueidentifier [note: 'Soft FK to identity.Users']
  CourseId uniqueidentifier [note: 'Soft FK to courses.Courses']
  EnrollmentDate datetime
  Status varchar // Active, Completed, Dropped
}

Table learning.TestAttempts {
  Id uniqueidentifier [pk]
  EnrollmentId uniqueidentifier [ref: > learning.Enrollments.Id]
  TestId uniqueidentifier [note: 'Soft FK to courses.Tests']
  Score int
  AttemptDate datetime
}
## Приклад EF Core конфігурації (Fluent API) для Soft FK

Щоб уникнути помилок міжмодульної взаємодії: **Soft FK налаштовується виключно як звичайна скалярна колонка**. 
НІЯКИХ викликів `HasOne()`, `WithMany()` або `HasForeignKey()` для зв'язків між різними схемами/модулями.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Courses.Domain.Entities;

namespace Courses.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        // Вказати назву таблиці та схему модуля
        builder.ToTable("Courses", "courses");
        
        builder.HasKey(x => x.Id);

        // ПРАВИЛЬНО: звичайна конфігурація колонки для Soft FK
        builder.Property(x => x.InstructorId)
               .IsRequired();
        
        // СУВОРО ЗАБОРОНЕНО (викличе помилку міграції, бо User в іншому контексті):
        // builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.InstructorId);

        builder.Property(x => x.Title)
               .HasMaxLength(200)
               .IsRequired();
    }
}