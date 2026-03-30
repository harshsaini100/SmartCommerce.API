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
        private readonly IUnitOfWork _unitOfWork;
        public CategoriesController(ICategoryRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync();

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
            var allCategories = await _repo.GetAllAsync();

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

            if (CreatesCycle(allCategories, id, dto.ParentId))
                return BadRequest("Circular hierarchy detected");

            category.Name = dto.Name;
            category.ParentCategoryId = dto.ParentId;

            _repo.Update(category);
            await _unitOfWork.SaveChangesAsync();

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
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree([FromQuery] int maxDepth = int.MaxValue)
        {
            var categories = await _repo.GetAllAsync();

           
            // Step 1: Prepare result list (root nodes)
            var rootNodes = new List<CategoryTreeDto>();

            // Step 1: Map to DTOs
            var dtoMap = categories.ToDictionary(
                c => c.Id,
                c => new CategoryTreeDto
                {
                    Id = c.Id,
                    Name = c.Name
                });

            // Step 3: Build tree
            foreach (var category in categories)
            {
                var dto = dtoMap[category.Id];

                if (category.ParentCategoryId == null)
                {
                    rootNodes.Add(dto);
                }
                else
                {
                    var parentDto = dtoMap[category.ParentCategoryId.Value];
                    parentDto.Children.Add(dto);
                }
            }

            // Apply depth limit
            foreach (var root in rootNodes)
            {
                TrimDepth(root, 1, maxDepth);
            }

            return Ok(rootNodes);
        }
        private void TrimDepth(CategoryTreeDto node, int currentDepth, int maxDepth)
        {
            if (currentDepth >= maxDepth)
            {
                node.Children.Clear();
                return;
            }

            foreach (var child in node.Children)
            {
                TrimDepth(child, currentDepth + 1, maxDepth);
            }
        }
        private bool CreatesCycle(List<Category> categories, int categoryId, int? newParentId)
        {
            if (newParentId == null)
                return false;

            var currentParentId = newParentId;

            while (currentParentId != null)
            {
                if (currentParentId == categoryId)
                    return true;

                currentParentId = categories
                    .First(c => c.Id == currentParentId)
                    .ParentCategoryId;
            }

            return false;
        }
    }
}
