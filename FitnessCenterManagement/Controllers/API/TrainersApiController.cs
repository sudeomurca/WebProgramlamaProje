using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models;

namespace FitnessCenterManagement.Controllers.API
{
    [Route("api/trainers")]
    [ApiController]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // Pazartesi gunu calisan antrenorleri listele
        [HttpGet("monday")]
        public async Task<ActionResult<IEnumerable<object>>> GetMondayTrainers()
        {
            // LINQ sorgusu ile Pazartesi calisanlari filtrele
            var mondayTrainers = await _context.TrainerAvailabilities
                .Where(ta => ta.DayOfWeek == DayOfWeekEnum.Monday && ta.IsActive)
                .Include(ta => ta.Trainer)
                .Select(ta => new
                {
                    TrainerId = ta.TrainerId,
                    TrainerName = ta.Trainer!.FirstName + " " + ta.Trainer.LastName,
                    Specialization = ta.Trainer.Specialization.ToString(),
                    Phone = ta.Trainer.Phone,
                    StartTime = ta.StartTime.ToString(@"HH:mm"),
                    EndTime = ta.EndTime.ToString(@"HH:mm"),
                    DayOfWeek = "Pazartesi"
                })
                .OrderBy(t => t.TrainerName)
                .ToListAsync();

            if (!mondayTrainers.Any())
            {
                return NotFound(new { message = "Pazartesi günü çalışan antrenör bulunamadı." });
            }

            return Ok(new
            {
                success = true,
                count = mondayTrainers.Count,
                data = mondayTrainers
            });
        }

       
        // Tum antrenorleri listele
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .Select(t => new
                {
                    Id = t.Id,
                    Name = t.FirstName + " " + t.LastName,
                    Specialization = t.Specialization.ToString(),
                    Phone = t.Phone,
                    Email = t.Email
                })
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = trainers.Count,
                data = trainers
            });
        }
    }
}
