using FitnessCenterManagement.Data;//db icin
using FitnessCenterManagement.Models;
using FitnessCenterManagement.Services;
using Microsoft.AspNetCore.Identity;//form islemleri icin
using Microsoft.EntityFrameworkCore;//db iletisimi icin

namespace FitnessCenterManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            var connectionString = builder.Configuration.GetConnectionString("SporSalonuBaglantisi") ??
                throw new InvalidOperationException("Hata: 'SporSalonuBaglantisi' bulunamadi.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            //e posta onayıyla ugrasmamak icin false
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            // gemini ai servisi - interface ile kaydediyoruz
            builder.Services.AddHttpClient<IAIService, CohereAIService>();

            var app = builder.Build();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Veritabani ve rolleri baslat
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                DbInitializer.Initialize(services).Wait();
            }
            app.Run();
        }
    }
}
