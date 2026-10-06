using System;
using System.Collections.Generic;

namespace Courses.Application.Test.DTOs;

public record QuestionResponseDto(Guid Id, string Text, int Points, IReadOnlyCollection<QuestionOptionResponseDto> Options);
