namespace RestaurantSystem.ViewModels.Kitchen
{
    public class KitchenOrderViewModel
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string OrderType { get; set; } = string.Empty;

        public int? TableNumber { get; set; }

        public List<KitchenOrderItemViewModel> Items { get; set; }
            = new List<KitchenOrderItemViewModel>();
    }
}