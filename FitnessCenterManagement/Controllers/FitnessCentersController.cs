using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FitnessCenterManagement.Controllers
{
    public class FitnessCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        //veritaban erisimi 
        public FitnessCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ
        // spor salonlarini db den cek view a gonder
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var centers = await _context.FitnessCenters.ToListAsync();
            return View(centers);
        }

        // url deki id ile uygun salonu eslestir bulursan view a gonder
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCenter = await _context.FitnessCenters
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fitnessCenter == null)
            {
                return NotFound();
            }

            return View(fitnessCenter);
        }


        // CREATE
        // form gonder
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // 
        [HttpPost]
        [ValidateAntiForgeryToken] //guvenlik
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(FitnessCenter fitnessCenter)
        {
            //validation kontrol
            if (ModelState.IsValid)
            {
                _context.Add(fitnessCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //invalid ise formu tekrarla
            return View(fitnessCenter);
        }


        // UPDATE
        // doldurulmus formu editleme/guncelleme icin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCenter = await _context.FitnessCenters.FindAsync(id);
            if (fitnessCenter == null)
            {
                return NotFound();
            }
            return View(fitnessCenter);
        }

        // guncelleme onaylama
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, FitnessCenter fitnessCenter)
        {
            if (id != fitnessCenter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fitnessCenter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessCenterExists(fitnessCenter.Id))
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
            return View(fitnessCenter);
        }

        // DELETE
        // silme islemi istegi 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCenter = await _context.FitnessCenters
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fitnessCenter == null)
            {
                return NotFound();
            }

            return View(fitnessCenter);
        }

        // silme istegini gerceklestirme
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessCenter = await _context.FitnessCenters.FindAsync(id);
            if (fitnessCenter != null)
            {
                _context.FitnessCenters.Remove(fitnessCenter);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FitnessCenterExists(int id)
        {
            return _context.FitnessCenters.Any(e => e.Id == id);
        }
    }
}