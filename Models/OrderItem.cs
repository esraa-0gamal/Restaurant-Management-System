using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
       public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        [StringLength(250)]
        public string? Notes { get; set; } // Optional note for the order item
        // Foreign key for Order
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        // Foreign key for Dish
        public int DishId { get; set; }
        public Dish? Dish { get; set; }

        public ICollection<OrderItemExtra> OrderItemExtras { get; set; }=new List<OrderItemExtra>();



    }
}
