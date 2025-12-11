using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenterManagement.Models
{
    public class AIRecommendation
    {
        public AIRecommendation()
        {
            UserId = "";
            Goal = "";
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arası olmalıdır")]
        public int? Height { get; set; }

        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arası olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Weight { get; set; }

        public string? BodyType { get; set; }

        [Required(ErrorMessage = "Hedef belirtilmelidir")]
        [StringLength(200)]
        public string Goal { get; set; }

        public string? Recommendation { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ImagePath { get; set; }
    }
}