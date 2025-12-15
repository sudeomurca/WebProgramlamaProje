using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int Id { get; set; }

        
        [Required(ErrorMessage = "Antrenör seçimi zorunludur")]
        public int TrainerId { get; set; }

        
        public Trainer? Trainer { get; set; }

        
        [Required(ErrorMessage = "Gün seçimi zorunludur")]
        public DayOfWeekEnum DayOfWeek { get; set; }

        
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        public TimeOnly StartTime { get; set; }

        
        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        public TimeOnly EndTime { get; set; }

       
        public bool IsActive { get; set; } = true;
    }
}