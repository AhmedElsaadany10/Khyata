using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Employee;
using Khyata.Domain.Enums;
using Khyata.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Khyata.Application.Exceptions.ExceptionError;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;

namespace Khyata.API.Controllers
{
    [Route("v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        /// <summary>
        /// Register a new workspace owner.
        /// Returns 202 — no token until an admin activates the workspace.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterOwnerDto dto)
        {
            var result = await _authRepository.RegisterOwnerAsync(dto);
            if (!result.IsSuccess) return this.ToActionResult(result);
            return Accepted(result.Value);
        }
        /// <summary>
        /// Login for owner and employees.
        /// Returns a JWT on success; 401/403 on bad credentials or inactive workspace.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authRepository.LoginAsync(dto);
            return this.ToActionResult(result);
        }
        
    }
}
