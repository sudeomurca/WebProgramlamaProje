using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class Service
    {
        public Service()
        {
            Name = "";
            IsActive = true;
        }
        public int Id { get; set; }

        
        [Required(ErrorMessage = "Hizmet adı zorunludur")]
        [StringLength(100)]
        public string Name { get; set; } 

        // Açıklama
        [StringLength(500)]
        public string? Description { get; set; }

        
        [Required(ErrorMessage = "Süre zorunludur")]
        [Range(15, 240, ErrorMessage = "Süre 15-240 dakika arası olmalıdır")]
        public int DurationMinutes { get; set; }

        
        [Required(ErrorMessage = "Ücret zorunludur")]
        [Range(0, 10000, ErrorMessage = "Ücret 0-10000 TL arası olmalıdır")]
        public decimal Price { get; set; }

        
        public bool IsActive { get; set; } 

        
        public int FitnessCenterId { get; set; }
        public FitnessCenter? FitnessCenter { get; set; }

        
        public ICollection<Appointment>? Appointments { get; set; }
    }
}