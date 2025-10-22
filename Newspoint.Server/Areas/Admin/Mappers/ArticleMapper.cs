using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;

namespace Newspoint.Server.Areas.Admin.Mappers;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<ArticleCreateDto, Article>()
            .ForMember(dest => dest.Comments, opt => opt.Ignore()) 
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());

        CreateMap<ArticleUpdateDto, Article>()
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());
    }
}