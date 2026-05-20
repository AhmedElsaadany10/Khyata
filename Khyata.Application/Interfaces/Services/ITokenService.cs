using khyata.Domain.Entities;

namespace khyata.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        int ExpiresInSeconds { get; }
    }
}
