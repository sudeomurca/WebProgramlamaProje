using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class AIRecommendation
    {
        public int Id { get; set; }

      
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yaş gerekli")]
        [Range(15, 100, ErrorMessage = "Yaş 15-100 arasında olmalıdır")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Boy gerekli")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır")]
        [Display(Name = "Boy (cm)")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Kilo gerekli")]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır")]
        [Display(Name = "Kilo (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi gerekli")]
        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hedef belirtilmeli")]
        [Display(Name = "Hedef")]
        public string Goal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Egzersiz seviyesi gerekli")]
        [Display(Name = "Egzersiz Seviyesi")]
        public string ExperienceLevel { get; set; } = string.Empty;

        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "AI Önerisi")]
        public string? Recommendation { get; set; } 
    }
}
