using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class FitnessCenter
    {
        public FitnessCenter()
        {
            Name = "";
            Address = "";
            Phone = "";
            Email = "";
            OpeningTime = "";
            ClosingTime = "";
            IsActive = true;
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Spor salonu adı zorunludur")]
        [StringLength(100, ErrorMessage = "Salon adı max 100 karakter olabilir")]
        public string Name { get; set; } 


        public string? Description { get; set; }


        [Required(ErrorMessage = "Adres zorunludur")]
        [StringLength(200)]
        public string Address { get; set; } 


        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; } 


        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } 


        [Required(ErrorMessage = "Açılış saati zorunludur")]
        public string OpeningTime { get; set; } 

        [Required(ErrorMessage = "Kapanış saati zorunludur")]
        public string ClosingTime { get; set; } 

        // Kapaliysa randevu alma
        public bool IsActive { get; set; } 

        
        public ICollection<Trainer>? Trainers { get; set; }

        
        public ICollection<Service>? Services { get; set; }
        
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
