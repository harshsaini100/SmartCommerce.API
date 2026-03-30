using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCommerce.API.DTOs.Product;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Repositories.Interfaces;

namespace SmartCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(
            IProductRepository repo,
            ICategoryRepository categoryRepo,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var categoryExists = await _categoryRepo.ExistsAsync(dto.CategoryId);

            if (!categoryExists)
                return BadRequest("Invalid CategoryId");

            var product = new Product
            {
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId
            };

            await _repo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repo.GetAllAsync();

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            });

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            // 🔥 Validate Category
            var categoryExists = await _categoryRepo.ExistsAsync(dto.CategoryId);

            if (!categoryExists)
                return BadRequest("Invalid CategoryId");

            // Update fields
            product.SKU = dto.SKU;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;

            _repo.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            product.IsActive = false;

            _repo.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
             [FromQuery] int? categoryId,
             [FromQuery] decimal? minPrice,
             [FromQuery] decimal? maxPrice)
        {
            var products = await _repo.SearchAsync(categoryId, minPrice, maxPrice);

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            });

            return Ok(result);
        }
    }
}
