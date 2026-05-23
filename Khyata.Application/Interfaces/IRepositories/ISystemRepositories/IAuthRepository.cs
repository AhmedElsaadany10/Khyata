using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Employee;
using Khyata.Application.Common;

namespace Khyata.Application.Interfaces.IRepositories.ISystemRepositories
{
    public interface IAuthRepository
    {

        Task<Result<RegisterResponseDto>> RegisterOwnerAsync(RegisterOwnerDto dto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    }
}
