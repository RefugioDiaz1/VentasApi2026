using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VentasApi2026.Common;
using VentasApi2026.Repositories.Interfaces;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly IProductService _service;

        public PagesController(IProductService service)
        {
            this._service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination)
        {
            var result = await _service.GetPagedAsync(pagination);

            var response = new
            {
                success = true,
                data = result.Items,
                meta = new
                {
                    result.Page,
                    result.PageSize,
                    result.TotalRecords,
                    result.TotalPages
                }
            };

            return Ok(response);
        }
    }
}
