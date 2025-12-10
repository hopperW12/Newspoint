using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Auth.DTOs;

namespace Newspoint.Server.Areas.Auth.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegisterDto, User>();
    }
}
