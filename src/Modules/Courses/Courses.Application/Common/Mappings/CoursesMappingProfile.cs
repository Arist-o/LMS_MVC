using AutoMapper;
using Courses.Application.Common.Models;
using Courses.Application.Course.DTOs;
using Courses.Application.Lesson.DTOs;
using Courses.Application.Review.DTOs;
using Courses.Application.Test.DTOs;

namespace Courses.Application.Common.Mappings;

public class CoursesMappingProfile : Profile
{
    public CoursesMappingProfile()
    {
        CreateMap<Domain.Entities.Course, CourseResponseDto>()
            .ForCtorParam("InstructorName", opt => opt.MapFrom(src => "Unknown")); // Since the mapping is replaced, we fallback to "Unknown" or it could be resolved differently.

        CreateMap<Domain.Entities.Course, LookupDto>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Name", opt => opt.MapFrom(src => src.Title));

        CreateMap<Domain.Entities.Lesson, LessonResponseDto>()
            .ForCtorParam("CourseTitle", opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : "Unknown"));
        CreateMap<Domain.Entities.Lesson, LookupDto>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Name", opt => opt.MapFrom(src => src.Title));

        CreateMap<Domain.Entities.Review, ReviewResponseDto>();

        CreateMap<Domain.Entities.Test, TestResponseDto>();
        CreateMap<Domain.Entities.Question, QuestionResponseDto>();
        CreateMap<Domain.Entities.QuestionOption, QuestionOptionResponseDto>();
    }
}
