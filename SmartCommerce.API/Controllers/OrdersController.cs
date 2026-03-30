using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCommerce.API.DTOs.Order;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Enums;
using SmartCommerce.API.Repositories.Interfaces;

namespace SmartCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Order must have at least one item");

            return Ok("Validation passed");
        }
    }
}
