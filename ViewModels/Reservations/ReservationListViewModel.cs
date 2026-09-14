namespace RestaurantSystem.ViewModels.Reservations
{
    public class ReservationListViewModel
    {
        public int ReservationId { get; set; }

        public DateTime ReservationDate { get; set; }

        public TimeSpan ReservationTime { get; set; }

        public int GuestCount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public int TableNumber { get; set; }

        public int TableCapacity { get; set; }

        public string LocationZone { get; set; } = string.Empty;
    }
}