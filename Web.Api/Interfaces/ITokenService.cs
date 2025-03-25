using Web.Api.Models;

namespace Web.Api.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user, CancellationToken cancellationToken);
    }
}