using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Learning.Application.Common.Interfaces;
using Learning.Application.Common.Models;
using Learning.Application.Enrollment.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Learning.Application.Enrollment.Queries;

public static class GetEnrollments
{
    public sealed record Query(Guid StudentId) : IRequest<Result<List<EnrollmentResponseDto>>>;

    public sealed class Handler(ILearningUnitOfWork unitOfWork) : IRequestHandler<Query, Result<List<EnrollmentResponseDto>>>
    {
        public async Task<Result<List<EnrollmentResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var enrollments = await unitOfWork.Enrollments.Query()
                .Where(e => e.StudentId == request.StudentId)
                .Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.EnrollmentDate, e.Status.ToString()))
                .ToListAsync(cancellationToken);

            return Result<List<EnrollmentResponseDto>>.Success(enrollments);
        }
    }
}
