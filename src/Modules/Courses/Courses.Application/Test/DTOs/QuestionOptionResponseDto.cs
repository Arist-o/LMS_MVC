using System;

namespace Courses.Application.Test.DTOs;

public record QuestionOptionResponseDto(Guid Id, string Text, bool IsCorrect);
