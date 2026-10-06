using System;
using System.Collections.Generic;

namespace Courses.Application.Test.DTOs;

public record AddOptionDto(string Text, bool IsCorrect);

public record AddQuestionToTestDto(Guid TestId, string Text, int Points, List<AddOptionDto> Options);
