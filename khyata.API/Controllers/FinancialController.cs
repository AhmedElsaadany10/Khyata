using Khyata.Application.DTOs.Order.Financial;
using Khyata.Application.Exceptions;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace khyata.API.Controllers
{
    [Route("v1/financial-report")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialRepository _financialRepository;
    
            public FinancialController(IFinancialRepository financialRepository)
            {
                _financialRepository = financialRepository;
            }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFinancialReport([FromQuery] FinancialReportQueryDto query)
        {
            if (User.GetRole() != WorkspaceRole.Owner.ToString())
                throw new ExceptionError.ForbiddenException("Only owner can see financial reports.");
            var result = await _financialRepository.GetReportAsync(User.GetWorkspaceId(), query);
            return this.ToActionResult(result);
        }
    }
}
