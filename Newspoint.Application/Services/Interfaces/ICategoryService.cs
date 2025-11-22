using Newspoint.Domain.Entities;

namespace Newspoint.Application.Services.Interfaces;

public interface ICategoryService : IService
{
    Task<ICollection<Category>> GetAll();
}
