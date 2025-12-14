using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad gereklidir")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad gereklidir")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uzmanlık alanı gereklidir")]
        public SpecializationType Specialization { get; set; }

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası gereklidir")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "Spor salonu seçilmelidir")]
        public int FitnessCenterId { get; set; }

        
        public FitnessCenter? FitnessCenter { get; set; } 
    }
}