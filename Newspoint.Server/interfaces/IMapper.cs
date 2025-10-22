namespace Newspoint.Server.Interfaces;

public interface IMapper<TSource, TDestination> where TDestination : IEntityDto
{
    TDestination Map(TSource entity);
    TSource MapBack(TDestination dto);
}