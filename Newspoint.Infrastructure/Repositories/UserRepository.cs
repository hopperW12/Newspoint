using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Repositories;

public interface IUserRepository : IRepository
{
    public Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
    Task<User?> Add(User user);
    Task<bool> Delete(int id);
}

public class UserRepository : IUserRepository
{
    private readonly DataDbContext _dataDbContext;

    public UserRepository(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public Task<User?> GetById(int id)
    {
        return _dataDbContext.Users.FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<User?> GetByEmail(string email)
    {
        return _dataDbContext.Users.FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<User?> Add(User entity)
    {
        await _dataDbContext.Users.AddAsync(entity);
        await _dataDbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _dataDbContext.Users.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return false;

        _dataDbContext.Users.Remove(entity);
        await _dataDbContext.SaveChangesAsync();
        return true;
    }
}