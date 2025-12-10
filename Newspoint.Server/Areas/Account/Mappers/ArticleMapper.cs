using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Account.DTOs;

namespace Newspoint.Server.Areas.Account.Mappers;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<AccountArticleCreateDto, Article>()
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());
        CreateMap<AccountArticleUpdateDto, Article>()
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());
    }
}
