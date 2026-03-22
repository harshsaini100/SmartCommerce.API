using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCommerce.API.DTOs.Category;
using SmartCommerce.API.Entities;
using SmartCommerce.API.Repositories.Interfaces;

namespace SmartCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repo;

        public CategoriesController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            
            if (dto.ParentId.HasValue)
            {
                var parentExists = await _repo.ExistsAsync(dto.ParentId.Value);

                if (!parentExists)
                    return BadRequest("Invalid ParentId");
            }

            var category = new Category
            {
                Name = dto.Name,
                ParentCategoryId = dto.ParentId
            };

            await _repo.AddAsync(category);
            await _repo.SaveChangesAsync();

            var result = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentCategoryId
            };

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repo.GetAllAsync();

            var result = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentCategoryId
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _repo.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            var result = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentCategoryId
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            var category = await _repo.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            // 🔥 Validate ParentId
            if (dto.ParentId.HasValue)
            {
                if (dto.ParentId.Value == id)
                    return BadRequest("Category cannot be its own parent");

                var parentExists = await _repo.ExistsAsync(dto.ParentId.Value);

                if (!parentExists)
                    return BadRequest("Invalid ParentId");
            }

            category.Name = dto.Name;
            category.ParentCategoryId = dto.ParentId;

            _repo.Update(category);
            await _repo.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repo.GetByIdAsync(id);

            if (category == null)
                return NotFound();
                        
            var hasChildren = await _repo.HasChildrenAsync(id);

            if (hasChildren)
                return BadRequest("Cannot delete category with children");

            _repo.Delete(category);
            await _repo.SaveChangesAsync();

            return NoContent();
        }
    }
}
