using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class CouponFormViewModel
    {
        public int CouponId { get; set; }

        [Required(ErrorMessage = "Coupon code is required")]
        [MaxLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        [Display(Name = "Coupon Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount amount is required")]
        [Range(1, 10000, ErrorMessage = "Discount must be greater than 0")]
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}