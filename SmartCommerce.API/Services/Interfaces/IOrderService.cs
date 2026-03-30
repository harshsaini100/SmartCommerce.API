using SmartCommerce.API.DTOs.Common;
using SmartCommerce.API.DTOs.Order;

namespace SmartCommerce.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderResultDto>> CreateOrderAsync(CreateOrderDto dto);
    }
}
