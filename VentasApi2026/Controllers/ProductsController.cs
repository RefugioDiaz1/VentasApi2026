using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentasApi2026.Common;
using VentasApi2026.Data;
using VentasApi2026.DTOs;
using VentasApi2026.Models;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service) {
            this._service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto) {

            var product = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = product.Id },
                ApiResponse<ProductDto>.Ok(product));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.getByIdAsync(id);

            return Ok(ApiResponse<ProductDto>.Ok(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, UpdateProductDto data)
        {
            var product = await _service.UpdateProduct(id, data);

            return Ok(ApiResponse<ProductDto>.Ok(product));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var product = await _service.DeleteById(id);

            return NoContent();
        }
    }
}
