using System;
using System.Collections.Generic;

namespace Learning.Application.TestAttempt.DTOs;

public class SubmitTestAttemptDto
{
    public Guid TestId { get; set; }
    public Dictionary<Guid, Guid> Answers { get; set; } = new();
}
