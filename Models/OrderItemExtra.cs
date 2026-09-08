using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class OrderItemExtra
    {
        [Key]
     public int OrderItemExtraId {  get; set; }

        //ForeignKey

        public int OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }
        public int ExtraId { get; set; }
        public Extra? Extra { get; set; }




    }
}
