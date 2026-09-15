using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using RestaurantSystem.ViewModels;
using RestaurantSystem.ViewModels.AccountViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantSystem.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(RestaurantDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region dashboard

        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalSales = await _context.Orders
                    .Where(o => o.Status == "Completed")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                OccupiedTables = await _context.RestaurantTables
                    .CountAsync(t => t.Status == "Occupied"),
                TotalTables = await _context.RestaurantTables.CountAsync(),
                PendingOrders = await _context.Orders
                    .CountAsync(o => o.Status == "Pending"),

                // جلب أحدث 10 طلبات مع بيانات العميل والطاولة
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Table)
                    .OrderByDescending(o => o.OrderDate)
                   
                    .ToListAsync()
            };

            return View("Dashboard", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Dashboard));
        }

        #endregion







        #region dish management

        [HttpGet]
        public async Task<IActionResult> Dishes()
        {
            var dishes = await _context.Dishes
                .Include(d => d.Category)
                .ToListAsync();

            return View("~/Views/Admin/Dishes/Dishes.cshtml", dishes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish == null) return NotFound();

            return View("~/Views/Admin/Dishes/Details.cshtml", dish);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new DishFormViewModel
            {
                Categories = await _context.Categories
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                    .ToListAsync()
            };
            return View("~/Views/Admin/Dishes/Create.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DishFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                    .ToListAsync();

                return View("~/Views/Admin/Dishes/Create.cshtml", vm);
            }

            string? imagePath = null;
            if (vm.ImageFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/dishes");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }
                imagePath = "/images/dishes/" + fileName;
            }

            var dish = new Dish
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                PreparationTime = vm.PreparationTime,
                IsAvailable = vm.IsAvailable,
                IsSoldOut = vm.IsSoldOut,
                CategoryId = vm.CategoryId,
                ImageUrl = imagePath
            };

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dishes));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            var vm = new DishFormViewModel
            {
                DishId = dish.DishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                PreparationTime = dish.PreparationTime,
                IsAvailable = dish.IsAvailable,
                IsSoldOut = dish.IsSoldOut,
                CategoryId = dish.CategoryId,
                ImageUrl = dish.ImageUrl,
                Categories = await _context.Categories
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                    .ToListAsync()
            };

            return View("~/Views/Admin/Dishes/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DishFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                    .ToListAsync();
                return View("~/Views/Admin/Dishes/Edit.cshtml", vm);
            }

            var dish = await _context.Dishes.FindAsync(vm.DishId);
            if (dish == null) return NotFound();

            if (vm.ImageFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/dishes");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }
                dish.ImageUrl = "/images/dishes/" + fileName;
            }

            dish.Name = vm.Name;
            dish.Description = vm.Description;
            dish.Price = vm.Price;
            dish.PreparationTime = vm.PreparationTime;
            dish.IsAvailable = vm.IsAvailable;
            dish.IsSoldOut = vm.IsSoldOut;
            dish.CategoryId = vm.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dishes));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish == null) return NotFound();
            return View("~/Views/Admin/Dishes/Delete.cshtml", dish);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dishes));
        }

        #endregion















        #region category management


        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Dishes)
                .ToListAsync();

            return View("~/Views/Admin/Categories/Categories.cshtml", categories);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            return View("~/Views/Admin/Categories/CategoryDetails.cshtml", category);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("~/Views/Admin/Categories/CreateCategory.cshtml", new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Categories/CreateCategory.cshtml", vm);

            string? imagePath = null;
            if (vm.ImageFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }
                imagePath = "/images/categories/" + fileName;
            }

            var category = new Category
            {
                Name = vm.Name,
                ImageUrl = imagePath
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryFormViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl
            };

            return View("~/Views/Admin/Categories/EditCategory.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Categories/EditCategory.cshtml", vm);

            var category = await _context.Categories.FindAsync(vm.CategoryId);
            if (category == null) return NotFound();

            if (vm.ImageFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }
                category.ImageUrl = "/images/categories/" + fileName;
            }

            category.Name = vm.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            return View("~/Views/Admin/Categories/DeleteCategory.cshtml", category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            if (category.Dishes != null && category.Dishes.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this category because it contains associated dishes!";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }


        #endregion













        #region table management

        [HttpGet]
        public async Task<IActionResult> Tables()
        {
            var tables = await _context.RestaurantTables
                .OrderBy(t => t.TableNumber)
                .ToListAsync();

            return View("Tables/Tables", tables);
        }

        [HttpGet]
        public IActionResult CreateTable()
        {
            return View("Tables/CreateTable", new TableFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTable(TableFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Tables/CreateTable", vm);
            }

            bool exists = await _context.RestaurantTables.AnyAsync(t => t.TableNumber == vm.TableNumber);
            if (exists)
            {
                ModelState.AddModelError("TableNumber", "A table with this number already exists.");
                return View("Tables/CreateTable", vm);
            }

            var table = new RestaurantTable
            {
                TableNumber = vm.TableNumber,
                Capacity = vm.Capacity,
                LocationZone = vm.LocationZone,
                Status = vm.Status
            };

            _context.RestaurantTables.Add(table);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tables));
        }

        [HttpGet]
        public async Task<IActionResult> EditTable(int id)
        {
            var table = await _context.RestaurantTables.FindAsync(id);
            if (table == null) return NotFound();

            var vm = new TableFormViewModel
            {
                TableId = table.TableId,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                LocationZone = table.LocationZone,
                Status = table.Status
            };

            return View("Tables/EditTable", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTable(TableFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Tables/EditTable", vm);
            }

            bool exists = await _context.RestaurantTables
                .AnyAsync(t => t.TableNumber == vm.TableNumber && t.TableId != vm.TableId);

            if (exists)
            {
                ModelState.AddModelError("TableNumber", "Another table already has this number.");
                return View("Tables/EditTable", vm);
            }

            var table = await _context.RestaurantTables.FindAsync(vm.TableId);
            if (table == null) return NotFound();

            table.TableNumber = vm.TableNumber;
            table.Capacity = vm.Capacity;
            table.LocationZone = vm.LocationZone;
            table.Status = vm.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tables));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.RestaurantTables.FindAsync(id);
            if (table == null) return NotFound();

            return View("Tables/DeleteTable", table);
        }

        [HttpPost, ActionName("DeleteTable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTableConfirmed(int id)
        {
            var table = await _context.RestaurantTables.FindAsync(id);
            if (table != null)
            {
                _context.RestaurantTables.Remove(table);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tables));
        }


        #endregion



























        #region users

        // 1. عرض كل المستخدمين مع أدوارهم
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "N/A",
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return View("Users/Users", model);
        }

        // 2. شاشة إضافة مستخدم وتحديد دوره
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var vm = new CreateUserViewModel
            {
                AvailableRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            };
            return View("Users/CreateUser", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                model.AvailableRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View("Users/CreateUser", model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                var roles = await _roleManager.Roles.ToListAsync();
                model.AvailableRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View("Users/CreateUser", model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // التأكد من وجود الرول وإسنادها له
                if (await _roleManager.RoleExistsAsync(model.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }

                return RedirectToAction(nameof(Users));
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            var availableRoles = await _roleManager.Roles.ToListAsync();
            model.AvailableRoles = availableRoles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
            return View("Users/CreateUser", model);
        }

        // 3. تعديل بيانات المستخدم ودوره
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.ToListAsync();

            var vm = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                SelectedRole = userRoles.FirstOrDefault() ?? "",
                AvailableRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            };

            return View("Users/EditUser", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                model.AvailableRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View("Users/EditUser", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.UserName = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var err in updateResult.Errors) ModelState.AddModelError(string.Empty, err.Description);
                var roles = await _roleManager.Roles.ToListAsync();
                model.AvailableRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
                return View("Users/EditUser", model);
            }

            // تحديث الرول (حذف الرولز القديمة وإسناد الرول الجديدة)
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (await _roleManager.RoleExistsAsync(model.SelectedRole))
            {
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            return RedirectToAction(nameof(Users));
        }

        #endregion










        #region Coupon Management

        [HttpGet]
        public async Task<IActionResult> Coupons()
        {
            var coupons = await _context.Coupons
                .OrderByDescending(c => c.ExpiryDate)
                .ToListAsync();

            return View("Coupons/Coupons", coupons);
        }

        [HttpGet]
        public IActionResult CreateCoupon()
        {
            return View("Coupons/CreateCoupon", new CouponFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoupon(CouponFormViewModel vm)
        {
            // فحص عدم تكرار الكود لأن عليه Unique Index في الداتابيز
            bool exists = await _context.Coupons.AnyAsync(c => c.Code.Trim().ToUpper() == vm.Code.Trim().ToUpper());
            if (exists)
            {
                ModelState.AddModelError("Code", "A coupon with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View("Coupons/CreateCoupon", vm);
            }

            var coupon = new Coupon
            {
                Code = vm.Code.Trim().ToUpper(),
                DiscountAmount = vm.DiscountAmount,
                ExpiryDate = vm.ExpiryDate,
                IsActive = vm.IsActive
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Coupons));
        }

        [HttpGet]
        public async Task<IActionResult> EditCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            var vm = new CouponFormViewModel
            {
                CouponId = coupon.CouponId,
                Code = coupon.Code,
                DiscountAmount = coupon.DiscountAmount,
                ExpiryDate = coupon.ExpiryDate,
                IsActive = coupon.IsActive
            };

            return View("Coupons/EditCoupon", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCoupon(CouponFormViewModel vm)
        {
            bool exists = await _context.Coupons
                .AnyAsync(c => c.Code.Trim().ToUpper() == vm.Code.Trim().ToUpper() && c.CouponId != vm.CouponId);

            if (exists)
            {
                ModelState.AddModelError("Code", "Another coupon with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View("Coupons/EditCoupon", vm);
            }

            var coupon = await _context.Coupons.FindAsync(vm.CouponId);
            if (coupon == null) return NotFound();

            coupon.Code = vm.Code.Trim().ToUpper();
            coupon.DiscountAmount = vm.DiscountAmount;
            coupon.ExpiryDate = vm.ExpiryDate;
            coupon.IsActive = vm.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Coupons));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            return View("Coupons/DeleteCoupon", coupon);
        }

        [HttpPost, ActionName("DeleteCoupon")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCouponConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Coupons));
        }

        #endregion




        #region exp

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedSampleOrders()
        {
            // 1. التأكد من وجود مستخدم لربطه بالطلب
            var user = await _userManager.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "customer@restaurant.com",
                    Email = "customer@restaurant.com",
                    FullName = "Ahmed Ali",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(user, "Password123!");
            }

            // 2. التأكد من وجود طاولة
            var table = await _context.RestaurantTables.FirstOrDefaultAsync();
            if (table == null)
            {
                table = new RestaurantTable
                {
                    TableNumber = 1,
                    Capacity = 4,
                    LocationZone = "Indoor",
                    Status = "Occupied"
                };
                _context.RestaurantTables.Add(table);
                await _context.SaveChangesAsync();
            }

            // 3. التأكد من وجود قسم وطبق
            var category = await _context.Categories.FirstOrDefaultAsync();
            if (category == null)
            {
                category = new Category { Name = "Main Courses" };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            var dish1 = await _context.Dishes.FirstOrDefaultAsync();
            if (dish1 == null)
            {
                dish1 = new Dish
                {
                    Name = "Grilled Chicken",
                    Price = 180.00m,
                    CategoryId = category.CategoryId,
                    IsAvailable = true,
                    PreparationTime = 25
                };
                _context.Dishes.Add(dish1);
                await _context.SaveChangesAsync();
            }

            // 4. إنشاء 3 طلبات تجريبية بحالات مختلفة
            var sampleOrders = new List<Order>
    {
        new Order
        {
            OrderNumber = "ORD-" + DateTime.Now.Ticks.ToString().Substring(12, 6),
            OrderDate = DateTime.Now.AddMinutes(-15),
            Status = "Pending",
            OrderType = "Dine-In",
            UserId = user.Id,
            TableId = table.TableId,
            SubTotal = 180.00m,
            TaxTotal = 25.20m,
            DiscountAmount = 0.00m,
            TotalAmount = 205.20m,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    DishId = dish1.DishId,
                    Quantity = 1,
                    UnitPrice = 180.00m,
                    TotalPrice = 180.00m
                }
            }
        },
        new Order
        {
            OrderNumber = "ORD-" + (DateTime.Now.Ticks + 1).ToString().Substring(12, 6),
            OrderDate = DateTime.Now.AddMinutes(-40),
            Status = "Cooking",
            OrderType = "Dine-In",
            UserId = user.Id,
            TableId = table.TableId,
            SubTotal = 360.00m,
            TaxTotal = 50.40m,
            DiscountAmount = 0.00m,
            TotalAmount = 410.40m,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    DishId = dish1.DishId,
                    Quantity = 2,
                    UnitPrice = 180.00m,
                    TotalPrice = 360.00m
                }
            }
        },
        new Order
        {
            OrderNumber = "ORD-" + (DateTime.Now.Ticks + 2).ToString().Substring(12, 6),
            OrderDate = DateTime.Now.AddHours(-2),
            Status = "Completed",
            OrderType = "Takeaway",
            DeliveryAddress = "Cairo, Nasr City",
            UserId = user.Id,
            SubTotal = 180.00m,
            TaxTotal = 25.20m,
            DiscountAmount = 0.00m,
            TotalAmount = 205.20m,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    DishId = dish1.DishId,
                    Quantity = 1,
                    UnitPrice = 180.00m,
                    TotalPrice = 180.00m
                }
            }
        }
    };

            _context.Orders.AddRange(sampleOrders);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        #endregion



    }
}
