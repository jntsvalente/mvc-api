using Microsoft.EntityFrameworkCore;
using Web.Api.Data.Mappings;
using Web.Api.Interfaces;
using Web.Api.Models;

namespace Web.Api.Data;
sealed class ApiDataContext(IConfiguration configuration) : DbContext, IApiDataContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql(configuration["Database:ConnectionString"]);

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.ApplyConfiguration(new ProductMap());
        model.ApplyConfiguration(new UserMap());
        model.ApplyConfiguration(new RoleMap());
        model.ApplyConfiguration(new UserRoleMap());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}