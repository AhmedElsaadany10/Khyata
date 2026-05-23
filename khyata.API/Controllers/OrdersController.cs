using Khyata.Application.DTOs.Order;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.API.Controllers
{
    [ApiController]
    [Route("v1/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        /// <summary>
        /// Create a new order. Available to both owners and employees.
        /// The creator is captured automatically from the JWT — no need to supply it.
        /// </summary>
        [HttpPost]
       // [Authorize(Policy = WorkspacePolicies.Employee)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var result = await _orderRepository.CreateAsync(
                User.GetWorkspaceId(), User.GetUserId(), dto);

            if (!result.IsSuccess) return this.ToActionResult(result);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// List orders with optional filters.
        /// Employees automatically see only the orders they created.
        /// Owners see all orders unless ?myOrders=true is passed.
        /// </summary>
        [HttpGet]
       // [Authorize(Policy = WorkspacePolicies.Employee)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] OrderQuery query)
        {
            var result = await _orderRepository.GetAllAsync(User.GetWorkspaceId(),User.GetUserId(),query);
            return this.ToActionResult(result);
        }
        /// <summary>Get a single order with customer info and creator details.</summary>
        [HttpGet("{id:guid}")]
        //[Authorize(Policy = WorkspacePolicies.Employee)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderRepository.GetByIdAsync(User.GetWorkspaceId(), id);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// Partially update an order.
        /// - Any field omitted (null) is left unchanged.
        /// - Status changes are validated against the transition rules.
        /// - Owners can update all fields; employees can only update status and payment.
        /// </summary>
        [HttpPatch("{id:guid}")]
        //[Authorize(Policy = WorkspacePolicies.Employee)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderDto dto)
        {

            var result = await _orderRepository.UpdateAsync(
                User.GetWorkspaceId(), id, User.GetUserId(), dto);

            return this.ToActionResult(result);
        }
    }
}
