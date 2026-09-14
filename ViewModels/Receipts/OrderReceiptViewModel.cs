namespace RestaurantSystem.ViewModels.Receipts
{
    public class OrderReceiptViewModel
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public string OrderType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int? TableNumber { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public decimal SubTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? CouponCode { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? PaymentDate { get; set; }

        public List<ReceiptItemViewModel> Items { get; set; }
            = new List<ReceiptItemViewModel>();
    }
}