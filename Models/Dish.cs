using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Dish
    {
        [Key]
        public int DishId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }=string.Empty;
        public string ?Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsSoldOut { get; set; }
        public int PreparationTime { get; set; } // Preparation time in minutes

        public bool IsAvailable { get; set; } // Indicates if the dish is available for ordering

        // Foreign key for Category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

       public ICollection<Extra> Extras { get; set; }=new List<Extra>(); // Many-to-many relationship with Extra
        public ICollection<OrderItem> OrderItems { get; set; } =new List<OrderItem>();// One-to-many relationship with OrderItem
         public ICollection<Review> Reviews {  get; set; }=new List<Review>();








    }
}
