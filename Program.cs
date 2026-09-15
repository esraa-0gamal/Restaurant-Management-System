using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using RestaurantSystem.Data;
using RestaurantSystem.Models;
using System.ClientModel;

namespace RestaurantSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DbContext
            builder.Services.AddDbContext<RestaurantDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<RestaurantDbContext>()
                .AddDefaultTokenProviders();

            // Login / Access Denied paths
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            //AI
            var openAiClient = new OpenAIClient(
                new ApiKeyCredential(
                    builder.Configuration.GetSection("AI")["ApiKey"]!),
                    new OpenAIClientOptions { Endpoint = new Uri(builder.Configuration.GetSection("AI")["BaseUrl"]!) }
                );

            var chatClient = openAiClient.GetChatClient(builder.Configuration.GetSection("AI")["Model"]!);

       
            builder.Services.AddSingleton<OpenAI.Chat.ChatClient>(chatClient);
            builder.Services.AddSingleton(chatClient);




         




            var app = builder.Build();

            // Error handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // 1. Static Files & Static Assets
            app.UseStaticFiles();
            

            // 2. Routing
            app.UseRouting();

            // 3. Session (يجب أن يكون بعد Routing وقبل Authentication)
            app.UseSession();

            // 4. Authentication & Authorization (يجب أن يكونوا بين Routing و MapControllerRoute)
            app.UseAuthentication();
            app.UseAuthorization();

            // 5. Endpoints / Map Controller Route (مرة واحدة فقط)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Menu}/{action=Index}/{id?}")
                .WithStaticAssets();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await RestaurantSystem.Data.DbInitializer.SeedAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }



            app.Run();
        }
    }
}