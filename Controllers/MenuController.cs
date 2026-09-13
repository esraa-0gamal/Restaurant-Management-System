using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Data;
using RestaurantSystem.Models;

namespace RestaurantSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly RestaurantDbContext context;
        public MenuController(RestaurantDbContext _context)
        {
            context = _context;
        }

        public async Task<IActionResult> Index(int? CategoryId, string searchTerm)
        {
            var categories = await context.Categories.ToListAsync();
            var dishes = context.Dishes.Include(d => d.Category).Where(d => d.IsAvailable);
            if (CategoryId.HasValue)
            {
                dishes = dishes.Where(d => d.CategoryId == CategoryId.Value);
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                dishes = dishes.Where(d => d.Name.Contains(searchTerm) || d.Description.Contains(searchTerm));
            }

            var extras = await context.Extras.ToListAsync();
            var veiwmodel = new MenuVeiwModel
            {
                Categories = categories,
                Dishes = await dishes.ToListAsync(),
                SelectedCategoryId = CategoryId,
                SearchTerm = searchTerm,
                Extras = extras
            };
            

            return View(veiwmodel);

        }


        public async Task<IActionResult> Details(int ?id)
        {
            if(id == null) return NotFound();
            var dish = await context.Dishes.Include(d => d.Category).FirstOrDefaultAsync(d => d.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }
    }
}