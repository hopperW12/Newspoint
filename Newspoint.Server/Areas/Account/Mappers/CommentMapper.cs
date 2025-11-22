using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Account.DTOs;

namespace Newspoint.Server.Areas.Account.Mappers;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CommentCreateDto, Comment>()
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Article, opt => opt.Ignore());
    }
}