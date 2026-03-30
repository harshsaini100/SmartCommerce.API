using SmartCommerce.API.Entities;

namespace SmartCommerce.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);

        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByIdsAsync(List<int> ids);

        void Update(Product product);

        void Delete(Product product);
        Task<List<Product>> SearchAsync(
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice);
    }
}
