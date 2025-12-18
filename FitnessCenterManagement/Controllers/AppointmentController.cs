using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenterManagement.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // db baglantisi
        public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // randevulari listele - Admin tum randevulari, Uye sadece kendi randevularini gorsun
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .ToListAsync();

            // Eger Admin degilse, sadece kendi randevularini goster
            if (!User.IsInRole("Admin"))
            {
                var userId = User.Identity.Name; // Email kullanicinin ID'si
                appointments = appointments.Where(a => a.UserId == userId).ToList();
            }

            // Her randevu icin kullanici bilgisini ekle
            var appointmentViewModels = new List<dynamic>();
            foreach (var appointment in appointments)
            {
                var user = await _userManager.FindByEmailAsync(appointment.UserId);
                appointmentViewModels.Add(new
                {
                    Appointment = appointment,
                    UserFullName = user != null ? $"{user.FirstName} {user.LastName}" : appointment.UserId
                });
            }

            ViewBag.AppointmentViewModels = appointmentViewModels;
            return View(appointments);
        }

        // randevu detay
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // randevu olusturma - Giris yapmis herkes randevu alabilir
        [Authorize]
        public IActionResult Create()
        {
            
            ViewBag.Trainers = _context.Trainers.ToList();
            ViewBag.Services = _context.Services.ToList();
            return View();
        }

        // randevu kaydetme
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.CreatedDate = DateTime.Now;
                appointment.Status = "Bekliyor";
                appointment.UserId = User.Identity.Name; // Kullanici email'ini otomatik ata

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Trainers = _context.Trainers.ToList();
            ViewBag.Services = _context.Services.ToList();
            return View(appointment);
        }

        // randevu duzenle - Sadece Admin duzenleyebilir (status degistirmek icin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.Trainers = _context.Trainers.ToList();
            ViewBag.Services = _context.Services.ToList();
            return View(appointment);
        }

        // randevu guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewBag.Services = _context.Services.ToList();
            return View(appointment);
        }

        // silme onay - Giris yapmis herkes silebilir
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // randevu silme islem
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        // Admin - Randevu Onayla
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Onaylandı";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Admin - Randevu Reddet
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Reddedildi";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}