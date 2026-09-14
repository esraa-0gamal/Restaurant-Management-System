namespace RestaurantSystem.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public Dish? Dish { get; set; }

        

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

      
        public decimal Discount { get; set; }

       
      
        public decimal TotalPrice => ( (UnitPrice - Discount) * Quantity);

    }
}
