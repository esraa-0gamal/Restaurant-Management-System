namespace RestaurantSystem.ViewModels.Kitchen
{
    public class KitchenOrderItemViewModel
    {
        public int OrderItemId { get; set; }

        public string DishName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string? Notes { get; set; }

        public int PreparationTime { get; set; }

        public List<string> Extras { get; set; } = new List<string>();
    }
}