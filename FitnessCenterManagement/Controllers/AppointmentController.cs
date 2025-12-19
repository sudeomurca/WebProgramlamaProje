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

        // randevu kaydetme - saat kontrolu ile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // AppointmentDate'in gun ve saat kismini ayir
                var appointmentTime = TimeOnly.FromDateTime(appointment.AppointmentDate);
                var dayOfWeekEnum = ConvertToDayOfWeekEnum(appointment.AppointmentDate.DayOfWeek);

                // Hizmet suresini al
                var service = await _context.Services
                    .Include(s => s.FitnessCenter)
                    .FirstOrDefaultAsync(s => s.Id == appointment.ServiceId);

                if (service == null)
                {
                    ModelState.AddModelError("", "Hizmet bulunamadı!");
                    ViewBag.Trainers = _context.Trainers.ToList();
                    ViewBag.Services = _context.Services.ToList();
                    return View(appointment);
                }

                // Randevunun bitis saatini hesapla
                var serviceDuration = TimeSpan.FromMinutes(service.DurationMinutes);
                var appointmentEndTime = appointmentTime.Add(serviceDuration);

                // 1. Antrenor musaitlik kontrolu - BASLANGIC VE BITIS SAATI
                var trainerAvailability = await _context.TrainerAvailabilities
                    .Where(ta => ta.TrainerId == appointment.TrainerId &&
                                 ta.DayOfWeek == dayOfWeekEnum)
                    .FirstOrDefaultAsync();

                if (trainerAvailability == null)
                {
                    ModelState.AddModelError("", "Seçilen antrenör bu gün müsait değil!");
                    ViewBag.Trainers = _context.Trainers.ToList();
                    ViewBag.Services = _context.Services.ToList();
                    return View(appointment);
                }

                // Baslangic saati kontrolu
                if (appointmentTime < trainerAvailability.StartTime)
                {
                    ModelState.AddModelError("", 
                        $"Antrenör bu saatte müsait değil. Müsait olduğu saatler: {trainerAvailability.StartTime:HH:mm} - {trainerAvailability.EndTime:HH:mm}");
                    ViewBag.Trainers = _context.Trainers.ToList();
                    ViewBag.Services = _context.Services.ToList();
                    return View(appointment);
                }

                // Bitis saati kontrolu - ONEMLI!
                var trainerEndTime = trainerAvailability.EndTime;
                if (trainerEndTime == TimeOnly.MinValue) // 00:00 ise, 23:59 kabul et
                {
                    trainerEndTime = new TimeOnly(23, 59);
                }

                if (appointmentEndTime > trainerEndTime)
                {
                    ModelState.AddModelError("", 
                        $"Randevu bitiş saati ({appointmentEndTime:HH:mm}) antrenörün müsait olduğu saatten ({trainerEndTime:HH:mm}) sonra! Lütfen daha erken bir saat veya daha kısa bir hizmet seçin.");
                    ViewBag.Trainers = _context.Trainers.ToList();
                    ViewBag.Services = _context.Services.ToList();
                    return View(appointment);
                }

                // 2. Spor salonu calisma saati kontrolu
                var fitnessCenter = service.FitnessCenter;
                
                // Kapanıs saati 00:00 ise, gece yarisi (24:00) anlamina gelir
                var closingTime = fitnessCenter.ClosingTime;
                if (closingTime == TimeOnly.MinValue) // 00:00 ise
                {
                    closingTime = new TimeOnly(23, 59); // 23:59 olarak kabul et
                }
                
                if (appointmentTime < fitnessCenter.OpeningTime ||
                    appointmentTime > closingTime)
                {
                    var displayClosingTime = fitnessCenter.ClosingTime == TimeOnly.MinValue ? "24:00" : closingTime.ToString(@"HH:mm");
                    ModelState.AddModelError("",
                        $"Spor salonu çalışma saatleri: {fitnessCenter.OpeningTime:HH:mm} - {displayClosingTime}");
                    ViewBag.Trainers = _context.Trainers.ToList();
                    ViewBag.Services = _context.Services.ToList();
                    return View(appointment);
                }

                // 3. Cakisan randevu kontrolu
                var appointmentEndDateTime = appointment.AppointmentDate.Add(serviceDuration);
                
                var conflictingAppointment = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.TrainerId == appointment.TrainerId &&
                                a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                                a.Status != "Reddedildi" &&
                                ((a.AppointmentDate >= appointment.AppointmentDate &&
                                  a.AppointmentDate < appointmentEndDateTime) ||
                                 (appointment.AppointmentDate >= a.AppointmentDate &&
                                  appointment.AppointmentDate < a.AppointmentDate.AddMinutes(a.Service.DurationMinutes))))
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                {
                    ModelState.AddModelError("", "Bu antrenörün seçilen saatte başka bir randevusu var!");
                    ViewBag.Trainers = _context.Trainers.ToList();
                    ViewBag.Services = _context.Services.ToList();
                    return View(appointment);
                }

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

        // Musait saat dilimlerini getir - AJAX icin
        [HttpGet]
        public async Task<JsonResult> GetAvailableTimeSlots(int trainerId, DateTime date)
        {
            // Antrenorun o gun icin musaitlik bilgisini al
            var dayOfWeekEnum = ConvertToDayOfWeekEnum(date.DayOfWeek);
            var availability = await _context.TrainerAvailabilities
                .Where(ta => ta.TrainerId == trainerId && ta.DayOfWeek == dayOfWeekEnum)
                .FirstOrDefaultAsync();

            if (availability == null)
            {
                return Json(new List<object>());
            }

            // O gun icin mevcut randevulari al
            var existingAppointments = await _context.Appointments
                .Include(a => a.Service)
                .Where(a => a.TrainerId == trainerId &&
                            a.AppointmentDate.Date == date.Date &&
                            a.Status != "Reddedildi")
                .ToListAsync();

            // Musait saat dilimlerini olustur (30 dakika araliklarla)
            var timeSlots = new List<object>();
            var currentTime = availability.StartTime;
            
            // EndTime 00:00 ise, 23:59 olarak kabul et
            var endTime = availability.EndTime;
            if (endTime == TimeOnly.MinValue)
            {
                endTime = new TimeOnly(23, 59);
            }

            while (currentTime < endTime)
            {
                // Bu saat diliminde randevu var mi kontrol et
                bool isAvailable = true;
                var currentDateTime = date.Date.Add(currentTime.ToTimeSpan());
                
                foreach (var apt in existingAppointments)
                {
                    var aptEndTime = apt.AppointmentDate.AddMinutes(apt.Service.DurationMinutes);
                    if (currentDateTime >= apt.AppointmentDate && currentDateTime < aptEndTime)
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    timeSlots.Add(new
                    {
                        value = currentTime.ToString(@"HH:mm"),
                        text = currentTime.ToString(@"HH:mm")
                    });
                }

                currentTime = currentTime.Add(TimeSpan.FromMinutes(30)); // 30 dakika ekle
            }

            return Json(timeSlots);
        }

        // DayOfWeek enum donusturme yardimci metod
        private DayOfWeekEnum ConvertToDayOfWeekEnum(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => DayOfWeekEnum.Monday,
                DayOfWeek.Tuesday => DayOfWeekEnum.Tuesday,
                DayOfWeek.Wednesday => DayOfWeekEnum.Wednesday,
                DayOfWeek.Thursday => DayOfWeekEnum.Thursday,
                DayOfWeek.Friday => DayOfWeekEnum.Friday,
                DayOfWeek.Saturday => DayOfWeekEnum.Saturday,
                DayOfWeek.Sunday => DayOfWeekEnum.Sunday,
                _ => DayOfWeekEnum.Monday
            };
        }

        // randevu duzenle - Admin veya randevu sahibi duzenleyebilir
        [Authorize]
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

            // Uye sadece kendi randevusunu duzenleyebilir ve sadece "Bekliyor" durumunda
            if (!User.IsInRole("Admin"))
            {
                if (appointment.UserId != User.Identity.Name)
                {
                    return Forbid(); // Baska kullanicinin randevusu
                }
                if (appointment.Status != "Bekliyor")
                {
                    TempData["Error"] = "Sadece 'Bekliyor' durumundaki randevular düzenlenebilir.";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Trainers = _context.Trainers.ToList();
            ViewBag.Services = _context.Services.ToList();
            return View(appointment);
        }

        // randevu guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            // Veritabanindan mevcut randevuyu al
            var existingAppointment = await _context.Appointments.FindAsync(id);
            if (existingAppointment == null)
            {
                return NotFound();
            }

            // Uye sadece kendi randevusunu duzenleyebilir ve sadece "Bekliyor" durumunda
            if (!User.IsInRole("Admin"))
            {
                if (existingAppointment.UserId != User.Identity.Name)
                {
                    return Forbid(); // Baska kullanicinin randevusu
                }
                if (existingAppointment.Status != "Bekliyor")
                {
                    TempData["Error"] = "Sadece 'Bekliyor' durumundaki randevular düzenlenebilir.";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut randevuyu guncelle
                    existingAppointment.TrainerId = appointment.TrainerId;
                    existingAppointment.ServiceId = appointment.ServiceId;
                    existingAppointment.AppointmentDate = appointment.AppointmentDate;
                    existingAppointment.Notes = appointment.Notes;
                    
                    // Admin status degistirebilir, uye degistiremez
                    if (User.IsInRole("Admin"))
                    {
                        existingAppointment.Status = appointment.Status;
                    }

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
