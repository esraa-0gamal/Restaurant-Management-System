using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.ViewModels
{
    public class ReservationFormViewModel
    {
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Reservation Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime ReservationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Reservation Time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public TimeSpan ReservationTime { get; set; } = DateTime.Now.TimeOfDay;

        [Required(ErrorMessage = "Guest count is required")]
        [Range(1, 50, ErrorMessage = "Guests must be between 1 and 50")]
        [Display(Name = "Number of Guests")]
        public int GuestCount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Table is required")]
        [Display(Name = "Table Number")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Display(Name = "Customer")]
        public string UserId { get; set; }

        public IEnumerable<SelectListItem>? Tables { get; set; }
        public IEnumerable<SelectListItem>? Users { get; set; }
    }
}