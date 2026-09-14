using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using System.Security.Claims;
using System.Text.Json;

namespace RestaurantSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly RestaurantDbContext context;

        public CartController(RestaurantDbContext context)
        {
            this.context = context;
        }

        private ExtraItemViewModel GetExtraFromSession()
        {
            var extraJson = HttpContext.Session.GetString("CartExtraItem");
            if (!string.IsNullOrEmpty(extraJson))
            {
                return JsonSerializer.Deserialize<ExtraItemViewModel>(extraJson);
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await context.CartItems.Include(c => c.Dish).ToListAsync();
            var extraItem = GetExtraFromSession();

            ViewBag.ExtraItem = extraItem;
            var totalPrice = cart.Sum(c => c.TotalPrice) + (extraItem?.TotalPrice ?? 0);

            return View((cart, totalPrice));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int dishId, int quantity = 1)
        {
            var dish = await context.Dishes.FindAsync(dishId);
            if (dish == null)
            {
                return NotFound();
            }

            var existingItem = await context.CartItems
                .FirstOrDefaultAsync(c => c.DishId == dishId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    DishId = dish.DishId,
                    UnitPrice = dish.Price,
                    Quantity = quantity,
                    Discount = 0
                };
                await context.CartItems.AddAsync(cartItem);
            }

            await context.SaveChangesAsync();

            return RedirectToAction("Details", "Menu", new { id = dishId });
        }

        [HttpPost]
        public async Task<IActionResult> AddExtraToCart(int extraId, int quantity = 1)
        {
            var extra = await context.Extras.FindAsync(extraId);
            if (extra == null)
            {
                return NotFound();
            }

            var extraItem = new ExtraItemViewModel
            {
                ExtraId = extra.ExtraId,
                Name = extra.Name,
                Price = extra.Price,
                Quantity = quantity
            };

            HttpContext.Session.SetString("CartExtraItem", JsonSerializer.Serialize(extraItem));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var item = await context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                context.CartItems.Remove(item);
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveExtra()
        {
            HttpContext.Session.Remove("CartExtraItem");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var cartItems = await context.CartItems.ToListAsync();

            if (cartItems.Any())
            {
                context.CartItems.RemoveRange(cartItems);
                await context.SaveChangesAsync();
            }

            HttpContext.Session.Remove("CartExtraItem");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var cartItems = await context.CartItems.Include(c => c.Dish).ToListAsync();
            var extraItem = GetExtraFromSession();

            if (!cartItems.Any() && extraItem == null)
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items to your cart before checking out.";
                return RedirectToAction(nameof(Index));
            }

            var subTotal = cartItems.Sum(c => c.TotalPrice) + (extraItem?.TotalPrice ?? 0);
            var taxTotal = subTotal * 0.15m;

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                SupTotal = subTotal,
                TaxTotal = taxTotal,
                TotalAmount = subTotal + taxTotal,
                AvailableTables = await context.RestaurantTables.Where(t => t.Status == "Available").ToListAsync()
            };

            ViewBag.ExtraItem = extraItem;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CheckoutViewModel model)
        {
            var cartItems = await context.CartItems.Include(c => c.Dish).ToListAsync();
            var extraItem = GetExtraFromSession();

            model.CartItems = cartItems;
            model.SupTotal = cartItems.Sum(c => c.TotalPrice) + (extraItem?.TotalPrice ?? 0);
            model.TaxTotal = model.SupTotal * 0.15m;
            model.AvailableTables = await context.RestaurantTables.Where(t => t.Status == "Available").ToListAsync();

            if (!string.IsNullOrEmpty(model.CouponCode))
            {
                var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code == model.CouponCode && c.IsActive);
                if (coupon != null)
                {
                    model.DiscountAmount = coupon.DiscountAmount;
                    model.TotalAmount = model.SupTotal + model.TaxTotal - model.DiscountAmount;
                    if (model.TotalAmount < 0) model.TotalAmount = 0;
                    model.AppliedCouponId = coupon.CouponId;
                }
                else
                {
                    ModelState.AddModelError("CouponCode", "Invalid coupon code.");
                    model.DiscountAmount = 0;
                    model.TotalAmount = model.SupTotal + model.TaxTotal;
                }
            }
            else
            {
                model.DiscountAmount = 0;
                model.TotalAmount = model.SupTotal + model.TaxTotal;
            }

            ViewBag.ExtraItem = extraItem;

            return View("CheckOut", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(CheckoutViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in or create an account first to complete your order.";
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await context.CartItems.Include(c => c.Dish).ToListAsync();
            var extraItem = GetExtraFromSession();

            if (!cartItems.Any() && extraItem == null)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            if (model.OrderType != "Delivery") ModelState.Remove("DeliveryAddress");
            if (model.OrderType != "Dine-in") ModelState.Remove("TableId");

            if (!ModelState.IsValid)
            {
                model.CartItems = cartItems;
                model.AvailableTables = await context.RestaurantTables.Where(t => t.Status == "Available").ToListAsync();
                ViewBag.ExtraItem = extraItem;
                return View("CheckOut", model);
            }

            int? selectedTableId = (model.OrderType == "Dine-in" && model.TableId > 0) ? model.TableId : null;
            int? selectedCouponId = (model.AppliedCouponId > 0) ? model.AppliedCouponId : null;

            var orderItemsList = cartItems.Select(c => new OrderItem
            {
                DishId = c.DishId,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                TotalPrice = c.TotalPrice
            }).ToList();

            if (extraItem != null)
            {
                var firstDishId = cartItems.FirstOrDefault()?.DishId ?? 1;

                orderItemsList.Add(new OrderItem
                {
                    DishId = firstDishId,
                    Quantity = extraItem.Quantity,
                    UnitPrice = extraItem.Price,
                    TotalPrice = extraItem.TotalPrice
                });
            }

            var order = new Order
            {
                UserId = userId,
                OrderType = model.OrderType,
                DeliveryAddress = model.OrderType == "Delivery" ? model.DeliveryAddress : null,
                TableId = selectedTableId,
                CouponId = selectedCouponId,
                SubTotal = model.SupTotal,
                DiscountAmount = model.DiscountAmount,
                TaxTotal = model.TaxTotal,
                TotalAmount = model.TotalAmount,
                OrderDate = DateTime.Now,
                Status = "Pending",
                OrderItems = orderItemsList
            };

            await context.Orders.AddAsync(order);

            if (cartItems.Any())
            {
                context.CartItems.RemoveRange(cartItems);
            }

            HttpContext.Session.Remove("CartExtraItem");

            await context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess", new { id = order.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
