using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Extra
    {
        [Key]
        public int ExtraId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public ICollection<Dish> Dishes { get; set; }=new List<Dish>();
        public ICollection<OrderItemExtra> OrderItemExtras { get; set; } = new List<OrderItemExtra>();


    }
}
