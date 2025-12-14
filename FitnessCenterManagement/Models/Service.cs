using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenterManagement.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet tipi gereklidir")]
        public ServiceType ServiceType { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Süre gereklidir")]
        [Range(15, 300, ErrorMessage = "Süre 15-300 dakika arasında olmalıdır")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Ücret gereklidir")]
        [Range(0.01, 10000, ErrorMessage = "Ücret 0.01-10000 TL arasında olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        
        [Required(ErrorMessage = "Spor salonu seçilmelidir")]
        public int FitnessCenterId { get; set; }

        
        public FitnessCenter? FitnessCenter { get; set; } 
    }
}