namespace SmartCommerce.API.DTOs.Order
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }
}
