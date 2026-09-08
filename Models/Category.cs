using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required,StringLength(100)]
        public string Name { get; set; }= string.Empty;

        public string ?ImageUrl { get; set; }


        public ICollection<Dish> Dishes { get; set; }=new List<Dish>();





    }
}
