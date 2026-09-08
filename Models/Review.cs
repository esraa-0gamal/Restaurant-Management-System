using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Review
    {
        [Key]
         public int ReviewId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        //Foreing Key
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int DishId { get; set; }
        public Dish? Dish { get; set; }



    }
}
