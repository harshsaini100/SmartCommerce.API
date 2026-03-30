using Microsoft.EntityFrameworkCore;
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

        public async Task<(bool Success, string Error, int OrderId, decimal TotalAmount)> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return (false, "Order must have at least one item", 0, 0);

            var productIds = dto.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            var products = await _productRepo.GetByIdsAsync(productIds);

            if (products.Count != productIds.Count)
                return (false, "One or more products do not exist", 0, 0);

            var productMap = products.ToDictionary(p => p.Id);

            foreach (var item in dto.Items)
            {
                var product = productMap[item.ProductId];

                if (!product.IsActive)
                    return (false, $"Product inactive: {product.Name}", 0, 0);

                if (item.Quantity <= 0)
                    return (false, $"Invalid quantity: {product.Name}", 0, 0);

                if (product.StockQuantity < item.Quantity)
                    return (false, $"Insufficient stock: {product.Name}", 0, 0);
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
                return (false, "Stock updated by another user. Retry.", 0, 0);
            }

            return (true, "", order.Id, order.TotalAmount);
        }
    }
}
