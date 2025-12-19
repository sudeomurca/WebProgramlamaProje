using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;

namespace FitnessCenterManagement.Controllers.API
{
    [Route("api/fitnesscenters")]
    [ApiController]
    public class FitnessCentersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FitnessCentersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/fitnesscenters/open-at-23
        // Saat 23:00'da hala acik olan spor salonlarini listele
        [HttpGet("open-at-23")]
        public async Task<ActionResult<IEnumerable<object>>> GetOpenAt23()
        {
            // 23:00 saati
            var targetTime = new TimeOnly(23, 0);

            // LINQ sorgusu ile 23:00'da acik olanlari filtrele
            var openCenters = await _context.FitnessCenters
                .Where(fc => fc.OpeningTime <= targetTime && 
                            (fc.ClosingTime >= targetTime || fc.ClosingTime == TimeOnly.MinValue))
                .Include(fc => fc.Services)
                .Select(fc => new
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Address = fc.Address,
                    Phone = fc.Phone,
                    OpeningTime = fc.OpeningTime.ToString(@"HH:mm"),
                    ClosingTime = fc.ClosingTime == TimeOnly.MinValue ? "24:00" : fc.ClosingTime.ToString(@"HH:mm"),
                    ServiceCount = fc.Services.Count,
                    Status = "23:00'da Açık"
                })
                .OrderBy(fc => fc.Name)
                .ToListAsync();

            if (!openCenters.Any())
            {
                return NotFound(new { message = "Saat 23:00'da açık spor salonu bulunamadı." });
            }

            return Ok(new
            {
                success = true,
                checkTime = "23:00",
                count = openCenters.Count,
                data = openCenters
            });
        }

        // GET: api/fitnesscenters/open-at/{hour}
        // Belirli bir saatte acik olan spor salonlarini listele (bonus - dinamik)
        [HttpGet("open-at/{hour}")]
        public async Task<ActionResult<IEnumerable<object>>> GetOpenAtHour(int hour)
        {
            if (hour < 0 || hour > 23)
            {
                return BadRequest(new { message = "Saat 0-23 arasında olmalıdır." });
            }

            var targetTime = new TimeOnly(hour, 0);

            var openCenters = await _context.FitnessCenters
                .Where(fc => fc.OpeningTime <= targetTime && 
                            (fc.ClosingTime >= targetTime || fc.ClosingTime == TimeOnly.MinValue))
                .Include(fc => fc.Services)
                .Select(fc => new
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Address = fc.Address,
                    Phone = fc.Phone,
                    OpeningTime = fc.OpeningTime.ToString(@"HH:mm"),
                    ClosingTime = fc.ClosingTime == TimeOnly.MinValue ? "24:00" : fc.ClosingTime.ToString(@"HH:mm"),
                    ServiceCount = fc.Services.Count,
                    Status = $"{hour:D2}:00'da Açık"
                })
                .OrderBy(fc => fc.Name)
                .ToListAsync();

            if (!openCenters.Any())
            {
                return NotFound(new { message = $"Saat {hour:D2}:00'da açık spor salonu bulunamadı." });
            }

            return Ok(new
            {
                success = true,
                checkTime = $"{hour:D2}:00",
                count = openCenters.Count,
                data = openCenters
            });
        }

        // GET: api/fitnesscenters
        // Tum spor salonlarini listele (bonus)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllFitnessCenters()
        {
            var centers = await _context.FitnessCenters
                .Include(fc => fc.Services)
                .Select(fc => new
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Address = fc.Address,
                    Phone = fc.Phone,
                    OpeningTime = fc.OpeningTime.ToString(@"HH:mm"),
                    ClosingTime = fc.ClosingTime == TimeOnly.MinValue ? "24:00" : fc.ClosingTime.ToString(@"HH:mm"),
                    ServiceCount = fc.Services.Count
                })
                .OrderBy(fc => fc.Name)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = centers.Count,
                data = centers
            });
        }
    }
}
