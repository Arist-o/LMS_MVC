---
name: dotnet-clean-architecture
description: Guidelines for building .NET projects using Clean Architecture, CQRS (MediatR), and Unit of Work. Use when the user asks to generate, refactor, or structure backend C# code.
---

# .NET Clean Architecture

1. **Ізоляція шарів**: Дотримуйся суворої ієрархії: Domain (чисті класи) -> Application (CQRS, Interfaces) -> Persistence (EF Core) / API.
2. **CQRS (Vertical Slice)**: Кожна операція — це окремий статичний клас, що містить `Command`/`Query`, `Validator` (FluentValidation) та `Handler`.
3. **Result Pattern**: Замість генерування `Exceptions` для бізнес-помилок, повертай з хендлерів обгортку `Result<T>`.
4. **Data Access (Управління даними)**:
   - Всі операції до SQL Server йдуть виключно через `IUnitOfWork` та `IRepository<T>`.
   - Не викликай `SaveChangesAsync()` в хендлерах (це робить TransactionBehavior).
5. **C# 12**: Використовуй Primary Constructors для ін'єкції залежностей (`public sealed class Handler(IUnitOfWork uow)`) та `record` для DTO і Команд/Запитів.