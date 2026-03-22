namespace SmartCommerce.API.DTOs.Category
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}
