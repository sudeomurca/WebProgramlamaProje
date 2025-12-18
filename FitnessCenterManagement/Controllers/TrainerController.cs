using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessCenterManagement.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // db baglantisi
        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // antrenorleri listele
        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            var trainers = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .ToListAsync();
            return View(trainers);
        }

        // antrenor detay
        [Authorize]
        public async Task<IActionResult> Details(int? id)
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

        // yeni antrenor ekle
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            
            ViewBag.FitnessCenters = _context.FitnessCenters.ToList();
            return View();
        }

        // yeni antrenor kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        // antrenor duzenle
        [Authorize(Roles = "Admin")]
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

        // antrenor guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        // antrenor silme onayi
        [Authorize(Roles = "Admin")]
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

        // antrenor sil
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        
        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}