---
name: aspnet-mvc-bootstrap
description: Guidelines for building ASP.NET Core MVC views using Bootstrap. Use when the user asks to create frontend views, UI components, or Razor pages.
---

# ASP.NET MVC & Bootstrap 5

1. **Структура View**: Працюй зі строго типізованими `ViewModel` (`@model MyViewModel`). Уникай `ViewBag`/`ViewData` для передачі основних даних.
2. **Tag Helpers**: Завжди використовуй вбудовані теги (`asp-for`, `asp-action`, `asp-controller`, `asp-validation-for`) замість ручного прописування HTML-атрибутів `name` чи `href`.
3. **Bootstrap 5**:
   - Макет: `.container`, `.row`, `.col-md-*`.
   - Форми: `.form-label`, `.form-control`, `.btn.btn-primary`.
   - Інтерфейс: використовуй компоненти `.card` для сутностей та `.alert` для сповіщень.
4. **Partial Views**: Винось повторюваний UI (наприклад, картку курсу чи відгуку) у Partial Views і викликай через `<partial name="_CourseCard" model="item" />`.
5. **Валідація**: Додавай `<span asp-validation-for="Property" class="text-danger"></span>` під інпутами та обов'язково підключай `_ValidationScriptsPartial` для роботи клієнтської валідації.