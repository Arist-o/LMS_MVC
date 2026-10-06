using AutoMapper;
using Identity.Application.Users.DTOs;
using Identity.Domain.Entities;

namespace Identity.Application.Common.Mappings;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
