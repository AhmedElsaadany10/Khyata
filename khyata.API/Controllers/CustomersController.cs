using Khyata.Application.DTOs.Customer.Requests;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.API.Controllers
{
    [ApiController]
    [Route("v1/customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        /// <summary>Create a new customer with an optional primary phone and measurements.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var result = await _customerRepository.CreateAsync(
                User.GetWorkspaceId(), User.GetUserId(), dto);

            if (!result.IsSuccess) return this.ToActionResult(result);
            return this.ToActionResult(result);

        }
        /// <summary>List customers with optional name/phone search and pagination.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] CustomerQuery query)
        {
            var result = await _customerRepository.GetAllAsync(User.GetWorkspaceId(), query);
            return this.ToActionResult(result);
        }
        /// <summary>Get a single customer with all phones and measurements.</summary>
        [HttpGet("{id:guid}", Name = nameof(GetById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _customerRepository.GetByIdAsync(User.GetWorkspaceId(), id);
            return this.ToActionResult(result);
        }
        /// <summary>Update customer name and/or measurements. Omit a field to leave it unchanged.</summary>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto dto)
        {
            var result = await _customerRepository.UpdateAsync(
                User.GetWorkspaceId(), id, User.GetUserId(), dto);
            return this.ToActionResult(result);
        }
        // ── Phone management ──────────────────────────────────────────────────────

        /// <summary>Add an additional phone number to a customer.</summary>
        [HttpPost("phones/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddPhone(Guid id, [FromBody] AddCustomerPhoneDto dto)
        {
            var result = await _customerRepository.AddPhoneAsync(User.GetWorkspaceId(), id, dto);
            return this.ToActionResult(result, successStatusCode: 201);
        }

        /// <summary>Remove a specific phone number from a customer. Cannot remove the last one.</summary>
        [HttpDelete("{id:guid}/phones/{phoneId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePhone(Guid id, Guid phoneId)
        {
            var result = await _customerRepository.RemovePhoneAsync(User.GetWorkspaceId(), id, phoneId);
            return this.ToActionResult(result);
        }

    }
}
