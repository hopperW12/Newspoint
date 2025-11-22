using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;
using Newspoint.Infrastructure.Database;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DataDbContext _dataDbContext;

    public CommentRepository(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public async Task<Comment?> GetById(int id)
    {
        return await _dataDbContext.Comments
            .Include(e => e.Author)
            .Include(e => e.Article)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Comment?> Add(Comment entity)
    {
        await _dataDbContext.Comments.AddAsync(entity);
        await _dataDbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<Comment?> Update(Comment entity)
    {
        _dataDbContext.Comments.Update(entity);
        await _dataDbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _dataDbContext.Comments.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return false;

        _dataDbContext.Comments.Remove(entity);
        await _dataDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Comment>> GetUserComments(int userId)
    {
        var comments = _dataDbContext.Comments
            .Where(e => e.AuthorId == userId)
            .Include(e => e.Author)
            .Include(e => e.Article)
            .ToListAsync();
        return await comments;
    }
}