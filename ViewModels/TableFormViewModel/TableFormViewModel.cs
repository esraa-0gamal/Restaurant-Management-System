using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class TableFormViewModel
    {
        public int TableId { get; set; }

        [Required(ErrorMessage = "Table Number is required")]
        [Display(Name = "Table Number")]
        public int TableNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50")]
        [Display(Name = "Seating Capacity")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Location Zone is required")]
        [MaxLength(50)]
        [Display(Name = "Zone / Area")]
        public string LocationZone { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Current Status")]
        public string Status { get; set; } = "Available";
    }
}