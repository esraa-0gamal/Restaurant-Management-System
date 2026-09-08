using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

     
        [Required]
      
      public string OrderNumber { get; set; } =Guid.NewGuid().ToString().Substring(0,8).ToUpper(); // Generate a unique order number
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // Default status
      

        public string OrderType { get; set; } = "Dine-in"; // New property for order type (e.g., "Dine-in", "Takeaway", "Delivery")
        public string? DeliveryAddress { get; set; }

         public string UserId { get; set; }= string.Empty; // Foreign key for ApplicationUser
        public ApplicationUser? User { get; set; }
        public int ?TableId { get; set; }
        public RestaurantTable? Table { get; set; } // Navigation property for related restaurant table

        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }
      
        public Payment? Payment { get; set; } //NOT a Collection Because every order has one payment
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Navigation property for related order items

    }
}
