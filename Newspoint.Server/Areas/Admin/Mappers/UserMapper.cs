using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;

namespace Newspoint.Server.Areas.Admin.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
    }
}