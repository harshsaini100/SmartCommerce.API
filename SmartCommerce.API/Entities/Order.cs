using SmartCommerce.API.Enums;

namespace SmartCommerce.API.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public List<OrderItem> Items { get; set; } = new();
}