using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Please select an order type.")]
        public string OrderType { get; set; } = "Dine-in";// Dine-in, Takeaway, Delivery
    
       public string ? DeliveryAddress { get; set; } // Only required for Delivery order type
       public int? TableId { get; set; }// Only required for Dine-in order type
        [StringLength(50)]
        public string? CustomerName { get; set; } // Optional for Takeaway and Delivery order types

        public string? CouponCode { get; set; } // Optional for all order types

        public int? AppliedCouponId { get; set; }

        public decimal SupTotal { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItem> CartItems { get; set; }= new List<CartItem>();
        public List<RestaurantTable> ?AvailableTables { get; set; } = new List<RestaurantTable>();
    }
}
