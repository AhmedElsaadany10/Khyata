using Khyata.Application.Common;
using Khyata.Application.DTOs.Order.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IRepositories.ISystemRepositories
{
    public interface IFinancialRepository
    {
        Task<Result<FinancialReportResponseDto>> GetReportAsync(
        Guid workspaceId, FinancialReportQueryDto query);
    }
}
