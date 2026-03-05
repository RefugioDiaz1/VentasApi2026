using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using VentasApi2026.Common;
using VentasApi2026.DTOs;
using VentasApi2026.Models;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        private readonly IOrderService _service;
        private readonly ICurrentUser _currentUser;

        public OrdersController(IOrderService service, ICurrentUser currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [Authorize(Policy = "CanCreateOrder")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var order = await _service.CreateAsync(dto, _currentUser.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                ApiResponse<OrderDto>.Ok(order));
        }

        [Authorize(Policy = "CanCompleteOrder")]
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteById(int id)
        {
            await _service.CompleteAsync(id, _currentUser.Id, _currentUser.Role);

            return NoContent();
        }

        [Authorize(Policy = "CanViewOrder")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var order = await _service.GetByIdAsync(id, _currentUser.Id, _currentUser.Role);

            return Ok(ApiResponse<OrderDto>.Ok(order));
        }

        [Authorize(Policy = "CanMyOrdersAsync")]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrdersAsync()
        {
            var order = await _service.GetMyOrdersAsync(_currentUser.Id, _currentUser.Role);

            return Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(order));
        }

        [Authorize(Policy = "CanViewAllOrder")]
        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var order = await _service.GetAllOrders();

            return Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(order));
        }

        [Authorize(Policy = "CanCancelOrder")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelById(int id)
        {
            await _service.CancelAsync(id, _currentUser.Id, _currentUser.Role);

            return NoContent();
        }

        [Authorize(Policy = "CanViewOrder")]
        [HttpGet]
        public async Task<IActionResult> GetOrders(
    [FromQuery] OrderQueryParameters query)
        {
            var result = await _service.GetFilteredAsync(query, _currentUser.Id, _currentUser.Role);

            return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
        }

    }
}
