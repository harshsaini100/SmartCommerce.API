using Microsoft.EntityFrameworkCore;
using SmartCommerce.API.Data;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Repositories.Interfaces;

namespace SmartCommerce.API.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
               .Where(p => p.IsActive) // important
               .Include(p => p.Category)
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<List<Product>> SearchAsync(
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
