namespace SmartCommerce.API.DTOs.Category
{
    public class CategoryTreeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<CategoryTreeDto> Children { get; set; } = new();
    }
}
