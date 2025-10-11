namespace Newspoint.Application.Mappers;

public interface IMapper<TSource, TDestination> 
{
    TDestination Map(TSource entity);
    TSource MapBack(TDestination dto);
}