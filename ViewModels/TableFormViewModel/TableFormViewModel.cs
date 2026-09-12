using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class TableFormViewModel
    {
        public int TableId { get; set; }

        [Required(ErrorMessage = "Table Number is required")]
        public int TableNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Location Zone is required")]
        [MaxLength(50)]
        public string LocationZone { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "Available";
    }
}