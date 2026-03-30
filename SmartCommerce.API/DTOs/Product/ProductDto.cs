namespace SmartCommerce.API.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string SKU { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
