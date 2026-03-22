namespace SmartCommerce.API.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}
