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
                    .CountAsync(o => o.Status == "Pending")
            };

            return View(viewModel);
        }










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









































        #region admin user management

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["SuccessMessage"] = "Admin user created successfully.";
                return RedirectToAction("Dashboard");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);


        }


        #endregion
    }
}
