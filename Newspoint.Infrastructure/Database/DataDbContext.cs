using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database.Seeders;

namespace Newspoint.Infrastructure.Database;

public class DataDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<User> Users { get; set; }

    public DataDbContext(DbContextOptions options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataDbContext).Assembly);
        
        // Init entities seeders
        modelBuilder.Entity<User>().HasData(new UserSeeder().GetEntities());
        modelBuilder.Entity<Category>().HasData(new CategorySeeder().GetEntities());
    }
}