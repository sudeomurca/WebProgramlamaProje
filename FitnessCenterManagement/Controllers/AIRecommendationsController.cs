using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;
using FitnessCenterManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [Authorize]
    public class AIRecommendationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIService _aiService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AIRecommendationsController(
            ApplicationDbContext context,
            IAIService aiService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _aiService = aiService;
            _userManager = userManager;
        }

        // yeni öneri isteme
        public IActionResult Create()
        {
            return View();
        }

        // AI dan öneri alma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AIRecommendation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        model.UserId = user.Id;
                        model.RequestDate = DateTime.Now;

                        
                        var prompt = $@"Kullanıcı Bilgileri:
- Yaş: {model.Age}
- Boy: {model.Height} cm
- Kilo: {model.Weight} kg
- Cinsiyet: {model.Gender}
- Hedef: {model.Goal}
- Egzersiz Seviyesi: {model.ExperienceLevel}

Lütfen bu kullanıcı için:
1. Kişiselleştirilmiş egzersiz programı öner
2. Beslenme tavsiyeleri ver
3. Hedefe ulaşmak için genel öneriler sun

Cevabını Türkçe olarak detaylı bir şekilde ver.";

                        model.Recommendation = await _aiService.GetRecommendationAsync(prompt);

                        // db a kaydet
                        _context.AIRecommendations.Add(model);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Details), new { id = model.Id });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "AI önerisi alınırken bir hata oluştu: " + ex.Message);
                }
            }

            return View(model);
        }

        // oneri detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recommendation = await _context.AIRecommendations.FindAsync(id);
            
            if (recommendation == null)
            {
                return NotFound();
            }

            
            var user = await _userManager.GetUserAsync(User);
            if (user != null && recommendation.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(recommendation);
        }

        
        public async Task<IActionResult> MyRecommendations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var recommendations = await _context.AIRecommendations
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(recommendations);
        }

        // admin icn oneriler sayfasi
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var recommendations = await _context.AIRecommendations
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(recommendations);
        }

        // oneri sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recommendation = await _context.AIRecommendations.FindAsync(id);
            
            if (recommendation == null)
            {
                return NotFound();
            }

            // uye sadece kendi onerisini siler
            var user = await _userManager.GetUserAsync(User);
            if (user != null && recommendation.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.AIRecommendations.Remove(recommendation);
            await _context.SaveChangesAsync();

            // adminse index e uyeyse onerilerime don
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(MyRecommendations));
        }

        // admin icin silme sayfasi
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recommendation = await _context.AIRecommendations.FindAsync(id);
            if (recommendation == null)
            {
                return NotFound();
            }

            return View(recommendation);
        }
    }
}
