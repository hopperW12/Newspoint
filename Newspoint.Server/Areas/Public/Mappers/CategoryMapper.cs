using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Server.Areas.Public.Mappers;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Comment, CommentDto>();
    }
}
