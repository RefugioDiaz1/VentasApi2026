using Microsoft.AspNetCore.Mvc;
using VentasApi2026.Common;
using VentasApi2026.DTOs;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var order = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                ApiResponse<OrderDto>.Ok(order));
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteById(int id)
        {
            await _service.CompleteAsync(id);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);

            return Ok(ApiResponse<OrderDto>.Ok(order));
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelById(int id)
        {
            await _service.CancelAsync(id);

            return NoContent();
        }

    }
}
