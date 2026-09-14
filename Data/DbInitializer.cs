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
           // await context.Database.EnsureDeletedAsync(); // Optional: Uncomment this line if you want to reset the database each time you run the application
          //  await context.Database.EnsureCreatedAsync();

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

            // 3. Seed Categories & Dishes
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
                            new Dish { Name = "Quattro Formaggi", Description = "Four cheese blend classic Italian style", Price = 260.00m, ImageUrl = "https://images.unsplash.com/photo-1573821663912-569905455b1c?w=800", PreparationTime = 18, IsAvailable = true }
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
                            new Dish { Name = "Classic Cheese Burger", Description = "Single beef patty, cheddar cheese, pickles, lettuce", Price = 160.00m, ImageUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?w=800", PreparationTime = 12, IsAvailable = true }
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
                            new Dish { Name = "Caesar Salad", Description = "Fresh romaine lettuce, croutons, and parmesan dressing", Price = 120.00m, ImageUrl = "https://www.spendwithpennies.com/wp-content/uploads/2023/06/Grilled-Chicken-Caesar-Salad-SpendWithPennies-4.jpg", PreparationTime = 10, IsAvailable = true },
                            new Dish { Name = "Penne Arrabbiata", Description = "Spicy tomato sauce with garlic and fresh parsley", Price = 165.00m, ImageUrl = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=800", PreparationTime = 18, IsAvailable = true }
                        }
                    },
                    new Category
                    {
                        Name = "Desserts & Drinks",
                        ImageUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=800",
                        Dishes = new List<Dish>
                        {
                            new Dish { Name = "Molten Lava Cake", Description = "Warm chocolate cake with a melting core", Price = 110.00m, ImageUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=800", PreparationTime = 12, IsAvailable = true },
                            new Dish { Name = "Fresh Mojito", Description = "Classic lime and fresh mint refresher", Price = 65.00m, ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=800", PreparationTime = 5, IsAvailable = true },
                            new Dish { Name = "Iced Spanish Latte", Description = "Espresso with condensed milk and chilled milk", Price = 75.00m, ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c?w=800", PreparationTime = 5, IsAvailable = true }
                        }
                    }
                };

                await context.Categories.AddRangeAsync(categories);
            }

            // 4. Seed Extras
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

            // 5. Seed Restaurant Tables
            if (!await context.RestaurantTables.AnyAsync())
            {
                var tables = new List<RestaurantTable>();
                for (int i = 1; i <= 12; i++)
                {
                    tables.Add(new RestaurantTable
                    {
                        TableNumber = i,
                        Capacity = (i % 3 == 0) ? 6 : (i % 2 == 0) ? 4 : 2,
                        LocationZone = (i <= 4) ? "Indoor" : (i <= 8) ? "Terrace" : "VIP Zone",
                        Status = "Available"
                    });
                }
                await context.RestaurantTables.AddRangeAsync(tables);
            }

            // 6. Seed Coupons
            if (!await context.Coupons.AnyAsync())
            {
                var coupons = new List<Coupon>();
                for (int i = 1; i <= 10; i++)
                {
                    coupons.Add(new Coupon
                    {
                        Code = $"DISCOUNT{i * 10}",
                        DiscountAmount = i * 15.00m,
                        ExpiryDate = DateTime.Now.AddMonths(2),
                        IsActive = true
                    });
                }
                await context.Coupons.AddRangeAsync(coupons);
            }

            await context.SaveChangesAsync();
        }
    }
}