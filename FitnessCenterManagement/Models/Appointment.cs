using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class Appointment
    {
        public Appointment()
        {
            UserId = "";
            Status = "Bekliyor";
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        
        [Required]
        public string UserId { get; set; }

        
        [Required(ErrorMessage = "Antrenör seçimi zorunludur")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        
        [Required(ErrorMessage = "Hizmet seçimi zorunludur")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        
        [Required(ErrorMessage = "Randevu tarihi zorunludur")]
        public DateTime AppointmentDate { get; set; }

        // Bekliyor, onaylandı, reddedildi, tamamlandı gibi
        public string Status { get; set; } 

        
        [StringLength(500)]
        public string? Notes { get; set; }

        
        public DateTime CreatedDate { get; set; } 

        
    }
}