using Khyata.Domain.Entities;

namespace Khyata.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        string GenerateToken(User user); 
        string GenerateAdminToken(AdminUser admin, IList<string> roles);
        int ExpiresInSeconds { get; }
    }
}
