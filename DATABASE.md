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