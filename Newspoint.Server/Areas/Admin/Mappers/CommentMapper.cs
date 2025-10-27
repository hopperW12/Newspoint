using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;

namespace Newspoint.Server.Areas.Admin.Mappers;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CommentCreateDto, Comment>()
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Article, opt => opt.Ignore());

        CreateMap<CommentUpdateDto, Comment>()
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Article, opt => opt.Ignore());
    }
}
