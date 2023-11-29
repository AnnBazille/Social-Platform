using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
    : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<Following> Followings { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Like> Likes { get; set; }

    public DbSet<Feed> Feeds { get; set; }
}
