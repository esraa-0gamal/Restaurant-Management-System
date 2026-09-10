using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class DishFormViewModel
    {
        public int DishId { get; set; }

        [Required(ErrorMessage = "اسم الطبق مطلوب")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.1, 10000, ErrorMessage = "السعر يجب أن يكون أكبر من 0")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Display(Name = "وقت التحضير (بالدقائق)")]
        public int PreparationTime { get; set; }

        [Display(Name = "متاح للطلب")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "نفذت الكمية")]
        public bool IsSoldOut { get; set; } = false;

        [Required(ErrorMessage = "يرجى اختيار القسم")]
        [Display(Name = "القسم")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}