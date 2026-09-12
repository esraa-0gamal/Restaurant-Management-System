using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class DishFormViewModel
    {
        public int DishId { get; set; }

        [Required(ErrorMessage = "dish name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "price is required")]
        [Range(0.1, 10000, ErrorMessage = "must be a positive value")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public int PreparationTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsSoldOut { get; set; } = false;

        [Required(ErrorMessage = "select a category")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}