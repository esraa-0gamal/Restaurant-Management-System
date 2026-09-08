using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required, StringLength(50)]
        public string Code { get; set; }=string.Empty;

        public decimal DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }=true;
        public ICollection<Order> Orders { get; set; } = new List<Order>();





    }
}
