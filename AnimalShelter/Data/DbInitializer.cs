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
                var animals = new List<Animal>
    {
        new Animal
        {
            Name = "Боби",
            Species = "Куче",
            Age = 3,
            Gender = Gender.Male,
            Status = AnimalStatus.ForAdoption,
            Description = "Дружелюбен, спокоен и много обича разходки.",
            HealthInfo = "Ваксиниран и обезпаразитен.",
            ImagePath = "/images/animals/default-dog.jpg"
        },
        new Animal
        {
            Name = "Мая",
            Species = "Котка",
            Age = 2,
            Gender = Gender.Female,
            Status = AnimalStatus.ForAdoption,
            Description = "Нежна и любопитна котка, която обича тиха среда.",
            HealthInfo = "Ваксинирана, обезпаразитена.",
            ImagePath = "/images/animals/default-cat.jpg"
        },
        new Animal
        {
            Name = "Рекс",
            Species = "Куче",
            Age = 5,
            Gender = Gender.Male,
            Status = AnimalStatus.ForAdoption,
            Description = "Енергичен и игрив, подходящ за активни стопани.",
            HealthInfo = "Кастриран, ваксиниран.",
            ImagePath = "/images/animals/default-dog.jpg"
        },
        new Animal
        {
            Name = "Лили",
            Species = "Котка",
            Age = 1,
            Gender = Gender.Female,
            Status = AnimalStatus.ForAdoption,
            Description = "Млада и игрива котка, лесно свиква с хора.",
            HealthInfo = "Здрава, обезпаразитена.",
            ImagePath = "/images/animals/cat1.jpg"
        },
        new Animal
        {
            Name = "Макс",
            Species = "Куче",
            Age = 4,
            Gender = Gender.Male,
            Status = AnimalStatus.ForAdoption,
            Description = "Лоялен пазач, но и много мил с познати хора.",
            HealthInfo = "Ваксиниран, с микрочип.",
            ImagePath = "/images/animals/default-dog.jpg"
        },
        new Animal
        {
            Name = "Снежи",
            Species = "Котка",
            Age = 3,
            Gender = Gender.Female,
            Status = AnimalStatus.ForAdoption,
            Description = "Спокойна котка, която обича да бъде галена.",
            HealthInfo = "Кастрирана, ваксинирана.",
            ImagePath = "/images/animals/cat2.jpg"
        },
        new Animal
        {
            Name = "Тара",
            Species = "Куче",
            Age = 2,
            Gender = Gender.Female,
            Status = AnimalStatus.ForAdoption,
            Description = "Умно и дружелюбно куче, подходящо за семейство.",
            HealthInfo = "Ваксинирана и обезпаразитена.",
            ImagePath = "/images/animals/default-dog.jpg"
        },
        new Animal
        {
            Name = "Оскар",
            Species = "Котка",
            Age = 6,
            Gender = Gender.Male,
            Status = AnimalStatus.ForAdoption,
            Description = "Спокоен котарак, подходящ за дом без много шум.",
            HealthInfo = "Кастриран, в добро здраве.",
            ImagePath = "/images/animals/default-cat.jpg"
        },
        new Animal
        {
            Name = "Зара",
            Species = "Куче",
            Age = 1,
            Gender = Gender.Female,
            Status = AnimalStatus.ForAdoption,
            Description = "Младо, весело куче, което обича внимание.",
            HealthInfo = "Ваксинирана.",
            ImagePath = "/images/animals/default-dog.jpg"
        },
        new Animal
        {
            Name = "Том",
            Species = "Котка",
            Age = 4,
            Gender = Gender.Male,
            Status = AnimalStatus.ForAdoption,
            Description = "Самостоятелен, но привързан към стопанина си.",
            HealthInfo = "Обезпаразитен, ваксиниран.",
            ImagePath = "/images/animals/default-cat.jpg"
        },
        new Animal
        {
            Name = "Рони",
            Species = "Куче",
            Age = 7,
            Gender = Gender.Male,
            Status = AnimalStatus.Adopted,
            Description = "Спокоен и възпитан, вече е намерил своя дом.",
            HealthInfo = "Ваксиниран, кастриран.",
            ImagePath = "/images/animals/default-dog.jpg"
        },
        new Animal
        {
            Name = "Пухи",
            Species = "Котка",
            Age = 5,
            Gender = Gender.Female,
            Status = AnimalStatus.Adopted,
            Description = "Много гальовна и социална, вече е осиновена.",
            HealthInfo = "Кастрирана, ваксинирана.",
            ImagePath = "/images/animals/default-cat.jpg"
        }
    };

                context.Animals.AddRange(animals);
                await context.SaveChangesAsync();
            }
        }
    }
}
