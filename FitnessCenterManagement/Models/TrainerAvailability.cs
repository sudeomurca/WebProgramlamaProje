using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class TrainerAvailability
    {
        public TrainerAvailability()
        {
            DayOfWeek = "";
            StartTime = "";
            EndTime = "";
            IsActive = true;
        }

        public int Id { get; set; }

        
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        
        [Required(ErrorMessage = "Gün seçimi zorunludur")]
        public string DayOfWeek { get; set; } 

        
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        public string StartTime { get; set; } 

        
        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        public string EndTime { get; set; } 

        
        public bool IsActive { get; set; } 
    }
}