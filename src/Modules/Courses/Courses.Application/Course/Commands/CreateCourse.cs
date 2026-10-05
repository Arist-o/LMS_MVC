using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Course.DTOs;
using Courses.Application.Review.DTOs;
using FluentValidation;
using MediatR;

namespace Courses.Application.Course.Commands
{
    public static class CreateCourse
    {
        public sealed record Command(CreateCourseDto Dto) : ICommand<Result<CourseResponseDto>>;

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Dto.InstructorId).NotEmpty().WithMessage("Instructor ID cannot be empty.");
                RuleFor(x => x.Dto.Title).NotEmpty().WithMessage("Title cannot be empty.");
                RuleFor(x => x.Dto.Description).NotEmpty().WithMessage("Description cannot be empty.");
            }
        }

        public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<CourseResponseDto>>
        {
            public async Task<Result<CourseResponseDto>> Handle(Command request , CancellationToken cancellationToken)
            {
                var course = new Domain.Entities.Course(
                    request.Dto.InstructorId, 
                    request.Dto.Title, 
                    request.Dto.Description, 
                    request.Dto.IsPublished);
                
                await unitOfWork.Courses.AddAsync(course, cancellationToken);
                return Result<CourseResponseDto>.Success(new CourseResponseDto(
                    course.Id,
                    course.InstructorId, 
                    course.Title, 
                    course.Description, 
                    course.IsPublished, 
                    course.CreatedAt));
            }
        }
    }

  
}
