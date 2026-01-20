using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;

namespace AnimalShelter.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1) Roles
            string[] roles = { "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2) Admin user (можеш да смениш имейла/паролата)
            var adminEmail = "admin@animalshelter.bg";
            var adminPassword = "Admin123!"; // после може да я смениш

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new Exception("Failed to create admin user: " + errors);
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");

            // 3) Seed sample animals (по желание)
            if (!await context.Animals.AnyAsync())
            {
                context.Animals.AddRange(
                    new Animal
                    {
                        Name = "Боби",
                        Species = "Куче",
                        Age = 3,
                        Gender = Gender.Male,
                        Description = "Много дружелюбен и игрив.",
                        HealthInfo = "Ваксиниран и обезпаразитен.",
                        ImagePath = "/images/animals/default-dog.jpg",
                        Status = AnimalStatus.ForAdoption
                    },
                    new Animal
                    {
                        Name = "Мая",
                        Species = "Котка",
                        Age = 2,
                        Gender = Gender.Female,
                        Description = "Спокойна и гальовна.",
                        HealthInfo = "Кастрирана.",
                        ImagePath = "/images/animals/default-cat.jpg",
                        Status = AnimalStatus.ForAdoption
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
