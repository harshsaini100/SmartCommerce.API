namespace SmartCommerce.API.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }

    public int? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }

    public List<Category> Children { get; set; } = new();
}