using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class RestaurantTable
    {
        [Key]
        public int TableId { get; set; }

        public int TableNumber { get; set; }

        public int Capacity { get; set; }
        [Required, StringLength(50)]
         public string LocationZone { get; set; }="Indoor"; // e.g., Indoor, Outdoor, VIP
        public string Status { get; set; }= "Available"; // e.g., Available, Reserved, Occupied

        public ICollection<Reservation> Reservations { get; set; }=new List<Reservation>();

        public ICollection<Order> Orders { get; set; }=new List<Order>();


    }
}
