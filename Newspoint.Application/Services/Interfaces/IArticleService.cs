using Newspoint.Domain.Entities;

namespace Newspoint.Application.Services.Interfaces;

public interface IArticleService : IService
{
    Task<ICollection<Article>> GetAll();
    Task<Result<Article>> GetById(int id);
    Task<Result<Article>> GetByIdWithComments(int id);
    Task<Result<Article>> Add(Article article);
    Task<Result<Article>> Update(Article article);
    Task<Result> Delete(int id);

    Task<ICollection<Article>> GetUserArticles(int userId);
    Task<Result> CanUserEdit(int userId, int articleId);
}
