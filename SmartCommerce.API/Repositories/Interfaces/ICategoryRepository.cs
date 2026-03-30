using SmartCommerce.API.Entities;

namespace SmartCommerce.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> AddAsync(Category category);

        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(int id);

        void Update(Category category);

        void Delete(Category category);
        Task<bool> HasChildrenAsync(int id);

        
    }
}
