using Microsoft.EntityFrameworkCore;
using SmartCommerce.API.DTOs.Common;
using SmartCommerce.API.DTOs.Order;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Enums;
using SmartCommerce.API.Repositories.Interfaces;
using SmartCommerce.API.Services.Interfaces;

namespace SmartCommerce.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<OrderResultDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return ServiceResult<OrderResultDto>.Failure("Order must have at least one item");

            var productIds = dto.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            var products = await _productRepo.GetByIdsAsync(productIds);

            if (products.Count != productIds.Count)
                return ServiceResult<OrderResultDto>.Failure("One or more products do not exist");

            var productMap = products.ToDictionary(p => p.Id);

            foreach (var item in dto.Items)
            {
                var product = productMap[item.ProductId];

                if (!product.IsActive)
                    return ServiceResult<OrderResultDto>.Failure($"Product inactive: {product.Name}");

                if (item.Quantity <= 0)
                    return ServiceResult<OrderResultDto>.Failure($"Invalid quantity: {product.Name}");

                if (product.StockQuantity < item.Quantity)
                    return ServiceResult<OrderResultDto>.Failure($"Insufficient stock: {product.Name}");
            }

            var order = new Order
            {
                UserId = dto.UserId,
                Status = OrderStatus.Pending
            };

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = productMap[item.ProductId];

                product.StockQuantity -= item.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                totalAmount += product.Price * item.Quantity;

                order.Items.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            await _orderRepo.AddAsync(order);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ServiceResult<OrderResultDto>.Failure("Stock updated by another user. Retry.");
            }
            return ServiceResult<OrderResultDto>.SuccessResult(new OrderResultDto { OrderId = order.Id, TotalAmount = order.TotalAmount });
        }
    }
}
