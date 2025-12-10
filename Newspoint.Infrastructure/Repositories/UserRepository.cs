using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataDbContext _dataDbContext;

    public UserRepository(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public async Task<ICollection<User>> GetAll()
    {
        var users = await _dataDbContext.Users.ToListAsync();
        return users;
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

    public async Task<User?> Update(User entity)
    {
        _dataDbContext.Users.Update(entity);
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
