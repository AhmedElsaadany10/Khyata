using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IRepositories.IAdminRepositories
{
    public interface IAdminAuthRepository
    {
        Task<Result<AdminLoginResponseDto>> LoginAsync(AdminLoginRequestDto dto);
        Task<Result<AdminProfileDto>> RegisterAsync(RegisterAdminDto dto);

    }
}
