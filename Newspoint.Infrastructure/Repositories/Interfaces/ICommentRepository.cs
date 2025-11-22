using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;

namespace Newspoint.Infrastructure.Repositories.Interfaces;

public interface ICommentRepository : IRepository
{
    Task<Comment?> GetById(int id);
    Task<Comment?> Add(Comment entity);
    Task<Comment?> Update(Comment entity);
    Task<bool> Delete(int id);
    
    Task<ICollection<Comment>> GetUserComments(int userId);
}