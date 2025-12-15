using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;

namespace FitnessCenterManagement.Controllers
{
    public class TrainerAvailabilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        // db  baglantisi
        public TrainerAvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // musaitlik listele
        public async Task<IActionResult> Index()
        {
            var availabilities = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .ToListAsync();
            return View(availabilities);
        }

        // musaitlik detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (availability == null)
            {
                return NotFound();
            }

            return View(availability);
        }

        // yeni musaitlik ekle
        public IActionResult Create()
        {
            // Dropdown için antrenörler listesi
            ViewBag.Trainers = _context.Trainers.ToList();
            return View();
        }

        // yeni musaitlik kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerAvailability availability)
        {
            if (ModelState.IsValid)
            {
                _context.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Trainers = _context.Trainers.ToList();
            return View(availability);
        }

        // musaitlik duzenle
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availability = await _context.TrainerAvailabilities.FindAsync(id);
            if (availability == null)
            {
                return NotFound();
            }

            ViewBag.Trainers = _context.Trainers.ToList();
            return View(availability);
        }

        // musaitlik guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerAvailability availability)
        {
            if (id != availability.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailabilityExists(availability.Id))
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

            ViewBag.Trainers = _context.Trainers.ToList();
            return View(availability);
        }

        // silme onay sayfasi
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (availability == null)
            {
                return NotFound();
            }

            return View(availability);
        }

        // musaitlik silme
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availability = await _context.TrainerAvailabilities.FindAsync(id);
            if (availability != null)
            {
                _context.TrainerAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
        private bool AvailabilityExists(int id)
        {
            return _context.TrainerAvailabilities.Any(e => e.Id == id);
        }
    }
}