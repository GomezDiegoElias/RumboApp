using Microsoft.EntityFrameworkCore;

namespace RumboApp.Identity.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    // public DbSet<User> Users => Set<User>();
}