namespace RestaurantSystem.Models
{


    public class ExtraItemViewModel 
    {public int ExtraId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    public decimal TotalPrice => Price * Quantity;

}
    public class CartViewModel
    {  
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<ExtraItemViewModel> ExtraItems { get; set; } = new List<ExtraItemViewModel>();

        public decimal GrandTotal => CartItems.Sum(item => item.TotalPrice) + ExtraItems.Sum(extra => extra.TotalPrice);

    }
}
