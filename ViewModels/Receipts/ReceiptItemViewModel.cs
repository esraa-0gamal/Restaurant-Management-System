namespace RestaurantSystem.ViewModels.Receipts
{
    public class ReceiptItemViewModel
    {
        public string DishName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Notes { get; set; }

        public List<string> Extras { get; set; } = new List<string>();
    }
}