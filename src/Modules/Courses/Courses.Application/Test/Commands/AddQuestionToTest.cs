using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Test.DTOs;
using Courses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Courses.Application.Test.Commands;

public static class AddQuestionToTest
{
    public sealed record Command(AddQuestionToTestDto Dto) : ICommand<Result<QuestionResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.TestId).NotEmpty().WithMessage("TestId is required.");
            RuleFor(x => x.Dto.Text).NotEmpty().WithMessage("Question text is required.");
            RuleFor(x => x.Dto.Points).GreaterThanOrEqualTo(0).WithMessage("Points cannot be negative.");
            RuleFor(x => x.Dto.Options).NotEmpty().WithMessage("At least one option is required.");
            RuleForEach(x => x.Dto.Options).ChildRules(options =>
            {
                options.RuleFor(o => o.Text).NotEmpty().WithMessage("Option text is required.");
            });
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<QuestionResponseDto>>
    {
        public async Task<Result<QuestionResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.Tests.GetByIdAsync(request.Dto.TestId, cancellationToken);
            if (test == null)
            {
                return Result<QuestionResponseDto>.Failure($"Test with ID {request.Dto.TestId} not found.");
            }

            var question = new Question(request.Dto.TestId, request.Dto.Text, request.Dto.Points);
            
            foreach (var optionDto in request.Dto.Options)
            {
                var option = new QuestionOption(question.Id, optionDto.Text, optionDto.IsCorrect);
                question.AddOption(option);
            }

            test.AddQuestion(question);
            await unitOfWork.Questions.AddAsync(question, cancellationToken);

            return Result<QuestionResponseDto>.Success(mapper.Map<QuestionResponseDto>(question));
        }
    }
}
