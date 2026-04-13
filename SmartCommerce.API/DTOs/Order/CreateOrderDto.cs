using System.ComponentModel.DataAnnotations;

namespace SmartCommerce.API.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "User id is required")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Send at leat one item")]
        public List<OrderItemDto> Items { get; set; }
    }
}
