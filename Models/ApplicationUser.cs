using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required,StringLength(100)]
        public string FullName { get; set; }=string.Empty;
        public ICollection<Order> Orders { get; set; }=new List <Order>();

        public ICollection<Reservation> Reservations { get; set; }= new List<Reservation>();

        public ICollection<Review> Reviews { get; set; }= new List<Review>();

    }
}
