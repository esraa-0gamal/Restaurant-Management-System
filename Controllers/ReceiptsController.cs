using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.ViewModels.Receipts;
using Microsoft.AspNetCore.Authorization;
namespace RestaurantSystem.Controllers
{
   // [Authorize(Roles = "Admin,Staff")]
    public class ReceiptsController : Controller
    {
        
        private readonly RestaurantDbContext _context;

        public ReceiptsController(RestaurantDbContext context)
        {
            _context = context;
        }

        // Customer receipt with prices, totals and payment details
        public async Task<IActionResult> CustomerReceipt(int orderId)
        {
            var receipt = await GetReceiptAsync(orderId);

            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // Kitchen receipt without financial information
        public async Task<IActionResult> KitchenReceipt(int orderId)
        {
            var receipt = await GetReceiptAsync(orderId);

            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // Shared method used by both receipt types
        private async Task<OrderReceiptViewModel?> GetReceiptAsync(int orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Select(o => new OrderReceiptViewModel
                {
                    OrderId = o.OrderId,

                    OrderNumber = o.OrderNumber,

                    OrderDate = o.OrderDate,

                    OrderType = o.OrderType,

                    Status = o.Status,

                    TableNumber = o.Table != null
                        ? o.Table.TableNumber
                        : null,

                    CustomerName = o.User != null
                        ? o.User.FullName
                        : "Unknown Customer",

                    SubTotal = o.SubTotal,

                    TaxTotal = o.TaxTotal,

                    DiscountAmount = o.DiscountAmount,

                    TotalAmount = o.TotalAmount,

                    CouponCode = o.Coupon != null
                        ? o.Coupon.Code
                        : null,

                    PaymentMethod = o.Payment != null
                        ? o.Payment.PaymentMethod
                        : null,

                    PaymentStatus = o.Payment != null
                        ? o.Payment.Status
                        : null,

                    PaymentDate = o.Payment != null
                        ? o.Payment.PaymentDate
                        : null,

                    Items = o.OrderItems
                        .Select(oi => new ReceiptItemViewModel
                        {
                            DishName = oi.Dish != null
                                ? oi.Dish.Name
                                : "Unknown Dish",

                            Quantity = oi.Quantity,

                            UnitPrice = oi.UnitPrice,

                            TotalPrice = oi.TotalPrice,

                            Notes = oi.Notes,

                            Extras = oi.OrderItemExtras
                                .Where(oie => oie.Extra != null)
                                .Select(oie => oie.Extra!.Name)
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}