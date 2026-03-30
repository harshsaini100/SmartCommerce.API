using SmartCommerce.API.DTOs.Order;

namespace SmartCommerce.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<(bool Success, string Error, int OrderId, decimal TotalAmount)> CreateOrderAsync(CreateOrderDto dto);
    }
}
