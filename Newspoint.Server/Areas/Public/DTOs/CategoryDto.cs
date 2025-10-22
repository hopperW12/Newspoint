using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Public.DTOs;

public class CategoryDto : IEntityDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}