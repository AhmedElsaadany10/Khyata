using Khyata.Application.DTOs.Workspace;
using Khyata.Application.Exceptions;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.API.Controllers
{
    [ApiController]
    [Route("v1/workspace")]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public WorkspacesController(IWorkspaceRepository workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }
       

        /// <summary>Returns the current user's workspace details including subscription dates.</summary>
        [HttpGet()]
       // [Authorize(Policy = WorkspacePolicies.OwnerOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWorkspace()
        {
            var result = await _workspaceRepository.GetMyWorkspaceAsync(User.GetWorkspaceId());
            return this.ToActionResult(result);
        }
        [HttpPatch("{workspaceId:guid}/name")]
        //[Authorize(Policy = WorkspacePolicies.OwnerOnly)]
        public async Task<IActionResult> UpdateName(Guid workspaceId, WorkspaceNameDto dto)
        {
            if (User.GetRole() != WorkspaceRole.Owner.ToString())
                throw new ExceptionError.ForbiddenException("Only owner can update workspace Name.");

            var result = await _workspaceRepository.UpdateNameAsync(workspaceId, dto);
            return this.ToActionResult(result);
        }
    }
}
