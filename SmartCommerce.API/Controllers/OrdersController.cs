using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCommerce.API.DTOs.Order;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Enums;
using SmartCommerce.API.Filters;
using SmartCommerce.API.Repositories.Interfaces;
using SmartCommerce.API.Services.Interfaces;

namespace SmartCommerce.API.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var result = await _orderService.CreateOrderAsync(dto);

            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
    }
}
