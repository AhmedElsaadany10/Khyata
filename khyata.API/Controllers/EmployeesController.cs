using Khyata.Shared.Pagination;
using khyata.Application.DTOs.Employee;
using khyata.Domain.Enums;
using khyata.Application.Extensions;
using khyata.Application.Interfaces.Repositories;
using khyata.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Khyata.Application.Exceptions.ExceptionError;
using Khyata.Application.Exceptions;

namespace khyata.API.Controllers
{
    [ApiController]
    [Route("v1/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public EmployeesController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
       
        /// <summary>Owner: get a single employee by id.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            if (User.GetRole() != UserRole.Owner.ToString())
                throw new ExceptionError.ForbiddenException("Only owners can view employee details.");

            var result = await _userRepository.GetEmployeeByIdAsync(User.GetWorkspaceId(), id);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// Owner: list all employees in the workspace (excludes soft-deleted by default).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            if (User.GetRole() != UserRole.Owner.ToString())
                throw new ExceptionError.ForbiddenException("Only owners can list employees.");

            var result = await _userRepository.GetEmployeesAsync(
                User.GetWorkspaceId(),
                new PaginationQuery { Page = page, Limit = limit });

            return this.ToActionResult(result);
        }
        /// <summary>
        /// Owner creates an employee account in their workspace.
        /// The employee is auto-linked to the owner's workspace via the JWT claim.
        /// </summary>
        [HttpPost()]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            if (User.GetRole() != UserRole.Owner.ToString())
                if (User.GetRole() != UserRole.Owner.ToString())
                    throw new ForbiddenException(
                        "Only workspace owners can create employee accounts.");
            var result = await _userRepository.CreateEmployeeAsync(User.GetWorkspaceId(), dto);
            return this.ToActionResult(result, successStatusCode: 201);
        }
        /// <summary>
        /// Owner: update an employee's name or password.
        /// Employee: update their own name or password (id must match the token sub).
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            // An employee can only update themselves
            if (User.GetRole() == UserRole.Employee.ToString() && User.GetUserId() != id)
                throw new ExceptionError.ForbiddenException("Employees can only update their own profile.");

            var result = await _userRepository.UpdateEmployeeAsync(User.GetWorkspaceId(), id, User.GetUserId(), dto);
            return this.ToActionResult(result);
        }
        /// <summary>Update the currently authenticated user's name or password.</summary>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateEmployeeDto dto)
        {
            var result = await _userRepository.UpdateOwnerAsync(User.GetUserId(), dto);

            return this.ToActionResult(result);
        }
        /// <summary>Owner: soft-delete an employee. Sets IsDeleted = true; never removes the row.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            if (User.GetRole() != UserRole.Owner.ToString())
                throw new ExceptionError.ForbiddenException("Only owners can deactivate employees.");

            var result = await _userRepository.DeleteEmployeeAsync(
                User.GetWorkspaceId(), id, User.GetUserId());

            return this.ToActionResult(result);
        }
    }
}
