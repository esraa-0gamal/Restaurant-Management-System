using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models;

namespace RestaurantSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

             await context.Database.MigrateAsync();
          

            // 1. Seed Roles
            string[] roles = { "Admin", "Staff", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admin User
            var adminEmail = "admin@restaurant.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Seed Categories & Dishes (تم توسيع الأطباق لتصل إلى 25 صنفاً موزعة على الأقسام مع الحفاظ على الأصول)
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Pizza",
                        ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=800",
                        Dishes = new List<Dish>
                        {
                            new Dish { Name = "Margherita Pizza", Description = "Classic tomato, fresh mozzarella, and basil", Price = 180.00m, ImageUrl = "https://images.unsplash.com/photo-1604382354936-07c5d9983bd3?w=800", PreparationTime = 15, IsAvailable = true },
                            new Dish { Name = "Pepperoni Feast", Description = "Loaded with spicy pepperoni and extra cheese", Price = 240.00m, ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=800", PreparationTime = 20, IsAvailable = true },
                            new Dish { Name = "BBQ Chicken Pizza", Description = "Grilled chicken, BBQ sauce, red onions", Price = 250.00m, ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=800", PreparationTime = 20, IsAvailable = true },
                            new Dish { Name = "Quattro Formaggi", Description = "Four cheese blend classic Italian style", Price = 260.00m, ImageUrl = "https://images.unsplash.com/photo-1573821663912-569905455b1c?w=800", PreparationTime = 18, IsAvailable = true },
                            new Dish { Name = "Vegetarian Supreme", Description = "Bell peppers, olives, mushrooms, onions, and corn", Price = 195.00m, ImageUrl = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=800", PreparationTime = 15, IsAvailable = true },
                            new Dish { Name = "Meat Lovers Pizza", Description = "Beef, pepperoni, sausage, and bacon chunks", Price = 280.00m, ImageUrl = "https://images.unsplash.com/photo-1534308983496-4fabb1a015ee?w=800", PreparationTime = 22, IsAvailable = true },
                            new Dish { Name = "Seafood Pizza", Description = "Shrimp, calamari, garlic, and mozzarella", Price = 300.00m, ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=800", PreparationTime = 25, IsAvailable = true }
                        }
                    },
                    new Category
                    {
                        Name = "Burgers",
                        ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800",
                        Dishes = new List<Dish>
                        {
                            new Dish { Name = "Truffle Smash Burger", Description = "Double beef patty, truffle mayo, aged cheddar", Price = 210.00m, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800", PreparationTime = 15, IsAvailable = true },
                            new Dish { Name = "Crispy Chicken Burger", Description = "Fried chicken breast, coleslaw, spicy sauce", Price = 170.00m, ImageUrl = "https://images.unsplash.com/photo-1625813506062-0aeb1d7a094b?w=800", PreparationTime = 15, IsAvailable = true },
                            new Dish { Name = "Mushroom Swiss Burger", Description = "Beef patty topped with sautéed mushrooms and Swiss cheese", Price = 220.00m, ImageUrl = "https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=800", PreparationTime = 18, IsAvailable = true },
                            new Dish { Name = "Classic Cheese Burger", Description = "Single beef patty, cheddar cheese, pickles, lettuce", Price = 160.00m, ImageUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?w=800", PreparationTime = 12, IsAvailable = true },
                            new Dish { Name = "Double Bacon Burger", Description = "Two patties, crispy beef bacon, smoky BBQ glaze", Price = 245.00m, ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?w=800", PreparationTime = 16, IsAvailable = true },
                            new Dish { Name = "Spicy Jalapeno Burger", Description = "Beef patty, fresh jalapenos, pepper jack cheese, spicy mayo", Price = 190.00m, ImageUrl = "https://www.chilitochoc.com/wp-content/uploads/2022/10/the-ultimate-cheeseburger-with-pickled-jalapenos.jpg", PreparationTime = 15, IsAvailable = true },
                            new Dish { Name = "Blue Cheese Burger", Description = "Juicy beef patty with rich blue cheese crumble", Price = 230.00m, ImageUrl = "https://www.deliciousmagazine.co.uk/wp-content/uploads/2023/07/2023D121_SMASHBURGERS_BEEF_1__.jpg", PreparationTime = 18, IsAvailable = true }
                        }
                    },
                    new Category
                    {
                        Name = "Pasta & Salads",
                        ImageUrl = "https://images.unsplash.com/photo-1645112411341-6c4fd023714a?w=800",
                        Dishes = new List<Dish>
                        {
                            new Dish { Name = "Fettuccine Alfredo", Description = "Creamy parmesan sauce with grilled chicken breast", Price = 195.00m, ImageUrl = "https://images.unsplash.com/photo-1645112411341-6c4fd023714a?w=800", PreparationTime = 22, IsAvailable = true },
                            new Dish { Name = "Spaghetti Bolognese", Description = "Classic rich meat sauce with slow-cooked tomatoes", Price = 185.00m, ImageUrl = "https://images.unsplash.com/photo-1551183053-bf91a1d81141?w=800", PreparationTime = 20, IsAvailable = true },
                            new Dish { Name = "Caesar Salad", Description = "Fresh romaine lettuce, croutons, and parmesan dressing", Price = 120.00m, ImageUrl = "https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=800", PreparationTime = 10, IsAvailable = true },
                            new Dish { Name = "Penne Arrabbiata", Description = "Spicy tomato sauce with garlic and fresh parsley", Price = 165.00m, ImageUrl = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=800", PreparationTime = 18, IsAvailable = true },
                            new Dish { Name = "Greek Salad", Description = "Cucumbers, tomatoes, feta cheese, olives, and olive oil", Price = 135.00m, ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=800", PreparationTime = 10, IsAvailable = true },
                            new Dish { Name = "Seafood Pasta", Description = "Linguine with shrimp, mussels, and white wine garlic sauce", Price = 290.00m, ImageUrl = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=800", PreparationTime = 25, IsAvailable = true },
                            new Dish { Name = "Pesto Penne", Description = "Fresh basil pesto, pine nuts, and parmesan flakes", Price = 175.00m, ImageUrl = "https://images.unsplash.com/photo-1621996346565-e3d5d6281229?w=800", PreparationTime = 15, IsAvailable = true }
                        }
                    },
                    new Category
                    {
                        Name = "Desserts & Drinks",
                        ImageUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=800",
                        Dishes = new List<Dish>
                        {
                            new Dish { Name = "Molten Lava Cake", Description = "Warm chocolate cake with a melting core", Price = 110.00m, ImageUrl = "https://sallysbakingaddiction.com/wp-content/uploads/2017/02/chocolate-molten-lava-cakes.jpg", PreparationTime = 12, IsAvailable = true },
                            new Dish { Name = "Fresh Mojito", Description = "Classic lime and fresh mint refresher", Price = 65.00m, ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=800", PreparationTime = 5, IsAvailable = true },
                            new Dish { Name = "Iced Spanish Latte", Description = "Espresso with condensed milk and chilled milk", Price = 75.00m, ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c?w=800", PreparationTime = 5, IsAvailable = true },
                            new Dish { Name = "Tiramisu", Description = "Traditional Italian coffee-flavored dessert", Price = 120.00m, ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=800", PreparationTime = 10, IsAvailable = true },
                            new Dish { Name = "Strawberry Cheesecake", Description = "Rich creamy cheesecake topped with fresh strawberry compote", Price = 130.00m, ImageUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?w=800", PreparationTime = 10, IsAvailable = true },
                            new Dish { Name = "Mango Smoothie", Description = "Thick blended fresh tropical mango fruit drink", Price = 80.00m, ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=800", PreparationTime = 5, IsAvailable = true },
                            new Dish { Name = "Blueberry Mojito", Description = "Refreshing soda infused with fresh blueberries and mint", Price = 70.00m, ImageUrl = "https://images.unsplash.com/photo-1536935338788-846bb9981813?w=800", PreparationTime = 5, IsAvailable = true },
                            new Dish { Name = "Chocolate Brownie", Description = "Fudgy warm brownie served with vanilla ice cream", Price = 95.00m, ImageUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=800", PreparationTime = 10, IsAvailable = true }
                        }
                    }
                };

                await context.Categories.AddRangeAsync(categories);
            }

            // 4. Seed Extras (بدون أي تعديل حسب رغبتك)
            if (!await context.Extras.AnyAsync())
            {
                var extras = new List<Extra>
                {
                    new Extra { Name = "Extra Cheese", Price = 25.00m },
                    new Extra { Name = "Jalapeno", Price = 15.00m },
                    new Extra { Name = "Mushroom", Price = 20.00m },
                    new Extra { Name = "Bacon", Price = 35.00m },
                    new Extra { Name = "Garlic Dip", Price = 15.00m },
                    new Extra { Name = "BBQ Sauce", Price = 15.00m },
                    new Extra { Name = "Ranch Sauce", Price = 15.00m },
                    new Extra { Name = "Caramelized Onions", Price = 18.00m },
                    new Extra { Name = "Extra Patty", Price = 60.00m },
                    new Extra { Name = "Avocado", Price = 30.00m }
                };
                await context.Extras.AddRangeAsync(extras);
            }

            // 5. Seed Restaurant Tables (توليد 25 ترابيزة بالضبط)
            if (!await context.RestaurantTables.AnyAsync())
            {
                var tables = new List<RestaurantTable>();
                for (int i = 1; i <= 25; i++)
                {
                    tables.Add(new RestaurantTable
                    {
                        TableNumber = i,
                        Capacity = (i % 3 == 0) ? 6 : (i % 2 == 0) ? 4 : 2,
                        LocationZone = (i <= 8) ? "Indoor" : (i <= 16) ? "Terrace" : "VIP Zone",
                        Status = "Available"
                    });
                }
                await context.RestaurantTables.AddRangeAsync(tables);
            }

            // 6. Seed Coupons (توليد 25 كوبون خصم بالضبط)
            if (!await context.Coupons.AnyAsync())
            {
                var coupons = new List<Coupon>();
                for (int i = 1; i <= 25; i++)
                {
                    coupons.Add(new Coupon
                    {
                        Code = $"SAVE{i * 5}",
                        DiscountAmount = (i * 5.00m),
                        ExpiryDate = DateTime.Now.AddMonths(3),
                        IsActive = true
                    });
                }
                await context.Coupons.AddRangeAsync(coupons);
            }

            await context.SaveChangesAsync();
        }
    }
}