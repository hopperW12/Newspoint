using Newspoint.Domain.Entities;

namespace Newspoint.Application.Services.Interfaces;

public interface ICommentService : IService
{
    Task<Result<Comment>> GetById(int id);
    Task<Result<Comment>> Add(Comment comment);
    Task<Result<Comment>> Update(Comment comment);
    Task<Result> Delete(int id);
}