using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Courses.Integration;
using Learning.Application.Common.Interfaces;
using Learning.Application.Common.Models;
using Learning.Application.Enrollment.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Learning.Application.Enrollment.Queries;

public static class GetEnrollments
{
    public sealed record Query(Guid StudentId) : IRequest<Result<List<EnrollmentResponseDto>>>;

    public sealed class Handler(ILearningUnitOfWork unitOfWork, IMapper mapper, ICourseIntegrationService courseIntegrationService) : IRequestHandler<Query, Result<List<EnrollmentResponseDto>>>
    {
        public async Task<Result<List<EnrollmentResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var enrollments = await unitOfWork.Enrollments.Query()
                .Where(e => e.StudentId == request.StudentId)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<EnrollmentResponseDto>>(enrollments);

            var courseIds = dtos.Select(e => e.CourseId).Distinct().ToList();
            if (courseIds.Any())
            {
                var testsByCourse = await courseIntegrationService.GetTestsForCoursesAsync(courseIds, cancellationToken);
                dtos = dtos.Select(dto => 
                {
                    if (testsByCourse.TryGetValue(dto.CourseId, out var tests))
                    {
                        return dto with { AvailableTests = tests.ToList() };
                    }
                    return dto;
                }).ToList();
            }

            return Result<List<EnrollmentResponseDto>>.Success(dtos);
        }
    }
}
