using AutoMapper;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Server.Areas.Public.Mappers;

public class CommentProfile : Profile
{
   public CommentProfile()
   {
      CreateMap<Comment, CommentDto>()
         .ForMember(dest => dest.Article, opt => opt.MapFrom(src => src.Article.Title)) 
         .ForMember(dest => dest.Author, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));
   }
}