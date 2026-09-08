using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
        public string Status { get; set; }= "Confirmed"; // e.g., Confirmed, Cancelled, Completed

        public int TableId { get; set; }
        public RestaurantTable ?Table {  get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser ?User { get; set; }


    }
}
