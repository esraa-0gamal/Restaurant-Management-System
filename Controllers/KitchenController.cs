using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Constants;
using RestaurantSystem.Data;
using RestaurantSystem.ViewModels.Kitchen;
using Microsoft.AspNetCore.Authorization;
namespace RestaurantSystem.Controllers
{
   // [Authorize(Roles = "Admin,Staff")]
    public class KitchenController : Controller
    {
        private readonly RestaurantDbContext _context;

        public KitchenController(RestaurantDbContext context)
        {
            _context = context;
        }

        // عرض الطلبات النشطة للمطبخ
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Status == OrderStatuses.Pending ||
                    o.Status == OrderStatuses.Preparing ||
                    o.Status == OrderStatuses.Ready)
                .OrderBy(o => o.OrderDate)
                .Select(o => new KitchenOrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    OrderType = o.OrderType,

                    TableNumber = o.Table != null
                        ? o.Table.TableNumber
                        : null,

                    Items = o.OrderItems
                        .Select(oi => new KitchenOrderItemViewModel
                        {
                            OrderItemId = oi.OrderItemId,

                            DishName = oi.Dish != null
                                ? oi.Dish.Name
                                : "Unknown Dish",

                            Quantity = oi.Quantity,

                            Notes = oi.Notes,

                            PreparationTime = oi.Dish != null
                                ? oi.Dish.PreparationTime
                                : 0,

                            Extras = oi.OrderItemExtras
                                .Where(oie => oie.Extra != null)
                                .Select(oie => oie.Extra!.Name)
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();

            return View(orders);
        }

        // Pending -> Preparing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartPreparing(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatuses.Pending)
            {
                TempData["ErrorMessage"] =
                    "Only pending orders can be started.";

                return RedirectToAction(nameof(Index));
            }

            order.Status = OrderStatuses.Preparing;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Order #{order.OrderNumber} is now being prepared.";

            return RedirectToAction(nameof(Index));
        }

        // Preparing -> Ready
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReady(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatuses.Preparing)
            {
                TempData["ErrorMessage"] =
                    "Only preparing orders can be marked as ready.";

                return RedirectToAction(nameof(Index));
            }

            order.Status = OrderStatuses.Ready;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Order #{order.OrderNumber} is ready.";

            return RedirectToAction(nameof(Index));
        }
    }
}