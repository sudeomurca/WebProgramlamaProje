using FitnessCenterManagement.Models;
using FitnessCenterManagement.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FitnessCenterManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Istatistikler
            ViewBag.TotalCenters = _context.FitnessCenters.Count();
            ViewBag.TotalTrainers = _context.Trainers.Count();
            ViewBag.TotalServices = _context.Services.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // API Test sayfasi
        public IActionResult ApiTest()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
