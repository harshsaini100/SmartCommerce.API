using Microsoft.EntityFrameworkCore;
using SmartCommerce.API.Data;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Repositories.Interfaces;

namespace SmartCommerce.API.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            return category;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories
                .AnyAsync(c => c.Id == id);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }
        public async Task<bool> HasChildrenAsync(int id)
        {
            return await _context.Categories
                .AnyAsync(c => c.ParentCategoryId == id);
        }
        
    }
}
