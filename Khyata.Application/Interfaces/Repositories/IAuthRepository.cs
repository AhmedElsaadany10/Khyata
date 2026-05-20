using khyata.Application.DTOs.Auth;
using khyata.Application.DTOs.Employee;
using Khyata.Application.Common;

namespace khyata.Application.Interfaces.Repositories
{
    public interface IAuthRepository
    {

        Task<Result<RegisterResponseDto>> RegisterOwnerAsync(RegisterOwnerDto dto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    }
}
