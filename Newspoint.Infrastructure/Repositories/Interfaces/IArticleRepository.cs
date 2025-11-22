using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;

namespace Newspoint.Infrastructure.Repositories.Interfaces;

public interface IArticleRepository : IRepository
{
    Task<ICollection<Article>> GetAll();
    Task<Article?> GetById(int id);
    Task<Article?> GetByIdWithComments(int id);
    Task<Article?> Add(Article entity);
    Task<Article?> Update(Article entity);
    Task<bool> Delete(int id);
    
    Task<ICollection<Article>> GetUserArticles(int userId);
}