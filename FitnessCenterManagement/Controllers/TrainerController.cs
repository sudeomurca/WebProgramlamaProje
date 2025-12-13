using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;

namespace FitnessCenterManagement.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // veritabanı bağlantısı için 
        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // tüm antrenorleri listele
        public async Task<IActionResult> Index()
        {
            // Antrenörleri ve hangi salona ait olduklarını getir
            var trainers = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .ToListAsync();
            return View(trainers);
        }

        // antrenor hakkinda
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // antrenor ve salon bilgisi
            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // antrenor ekle
        public IActionResult Create()
        {
            // Dropdown için spor salonları listesi
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View();
        }

        // antrenor kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View(trainer);
        }

        // antrenor düzenle
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View(trainer);
        }

        // antrenor güncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer)
        {
            if (id != trainer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                    if (!TrainerExists(trainer.Id))
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
            return View(trainer);
        }

        // silme onayi
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // antrenor silme 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // antrenor var mi
        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}