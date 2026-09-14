using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using System.Security.Claims;

namespace RestaurantSystem.Controllers
{

    public class OrderController : Controller
    {
        private readonly RestaurantDbContext context;

        public OrderController(RestaurantDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

       
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



    }
}
