using khyata.Application.Extensions;
using khyata.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace khyata.API.Controllers
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
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWorkspace()
        {
            var result = await _workspaceRepository.GetMyWorkspaceAsync(User.GetWorkspaceId());
            return this.ToActionResult(result);
        }
    }
}
