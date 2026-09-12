using RestaurantSystem.Models;
using System.Collections.Generic;

namespace RestaurantSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int OccupiedTables { get; set; }
        public int TotalTables { get; set; }
        public int PendingOrders { get; set; }

        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}