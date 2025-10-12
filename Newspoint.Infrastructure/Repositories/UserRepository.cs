using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Repositories;

public interface IUserRepository : IRepository
{
    public Task<User?> GetById(int id);
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
}