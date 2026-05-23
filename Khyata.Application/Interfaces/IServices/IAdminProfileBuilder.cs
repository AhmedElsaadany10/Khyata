using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IServices
{
    public interface IAdminProfileBuilder
    {
        Task<AdminProfileDto> BuildAsync(AdminUser user);
    }
}
