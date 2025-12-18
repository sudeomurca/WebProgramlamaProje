using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessCenterManagement.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        // veritabani baglantisi icin 
        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // tum hizmetleri listele
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Hizmetleri ve hangi salona ait olduklarını getir
            var services = await _context.Services
                .Include(s => s.FitnessCenter)
                .ToListAsync();
            return View(services);
        }

        // hizmet detaylari
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // yeni hizmet ekle
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View();
        }

        // yeni hizmet kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View(service);
        }

        // hizmet duzenleme sayfasi 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View(service);
        }

        // hizmet guncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View(service);
        }

        // silme onay sayfasi 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // hizmet silme
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}