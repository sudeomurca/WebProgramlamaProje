using Microsoft.AspNetCore.Identity;//rol yonetimi
using FitnessCenterManagement.Models;

namespace FitnessCenterManagement.Data
{
    //rolleri ve admin hesabini db e eklemek icin
    public static class DbInitializer //nesne olusturmadan cagir
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            //gerekli servisleri alma
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //roller
            string[] roleNames = { "Admin", "Uye" };
            //roller db de mevcut mu
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //admin bilgileri
            var adminEmail = "g231210027@sakarya.edu.tr"; 
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Yönetici"
                };

                var result = await userManager.CreateAsync(newAdmin, "Sau123!");
                //basarili ise admin yetkisi ver
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}