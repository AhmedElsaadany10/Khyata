using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Auth;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.Admin.Controllers
{
    [ApiController]
    [Route("admin/auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAdminAuthRepository _authRepository;

        public AdminAuthController(IAdminAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        /// <summary>
        /// Admin login. Returns an admin-scoped JWT.
        /// This token is cryptographically rejected by the main API (different audience).
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AdminLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestDto dto)
        {
            var result = await _authRepository.LoginAsync(dto);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// Register a new admin user (SuperAdmin or Moderator).
        /// Requires SuperAdmin role.
        /// </summary>
        [HttpPost("register")]
        //[Authorize(Policy = AdminPolicies.SuperAdminOnly)]
        [ProducesResponseType(typeof(AdminProfileDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterAdminDto dto)
        {
            var result = await _authRepository.RegisterAsync(dto);
            return this.ToActionResult(result);
        }
    }
}
