using AutoMapper;
using Learning.Application.Enrollment.DTOs;
using Learning.Application.TestAttempt.DTOs;

namespace Learning.Application.Common.Mappings;

public class LearningMappingProfile : Profile
{
    public LearningMappingProfile()
    {
        CreateMap<Domain.Entities.Enrollment, EnrollmentResponseDto>();
        CreateMap<Domain.Entities.TestAttempt, TestAttemptResponseDto>();
    }
}
