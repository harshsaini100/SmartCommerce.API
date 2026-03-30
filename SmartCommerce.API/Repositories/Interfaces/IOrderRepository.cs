using SmartCommerce.API.Entities;

namespace SmartCommerce.API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
    }
}
