using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Category Image")]
        public IFormFile? ImageFile { get; set; }
    }
}