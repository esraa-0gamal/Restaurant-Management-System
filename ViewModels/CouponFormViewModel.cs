using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class CouponFormViewModel
    {
        public int CouponId { get; set; }

        [Required(ErrorMessage = "Coupon code is required")]
        [MaxLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount amount is required")]
        [Range(1, 10000, ErrorMessage = "Discount must be greater than 0")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; } = DateTime.Today.AddDays(7);

        public bool IsActive { get; set; } = true;
    }
}