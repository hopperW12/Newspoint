﻿using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Repositories;

public interface ICommentRepository : IRepository
{
    Task<Comment?> GetById(int id);
    Task<Comment?> Add(Comment entity);
    Task<Comment?> Update(Comment entity);
    Task<bool> Delete(int id);
}

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
}