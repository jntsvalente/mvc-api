using Microsoft.EntityFrameworkCore;
using Web.Api.Models;

namespace Web.Api.Interfaces
{
    public interface IApiDataContext
    {
        DbSet<Product> Products { get; }
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}