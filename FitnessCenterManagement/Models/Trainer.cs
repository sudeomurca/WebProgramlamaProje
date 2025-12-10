using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public class Trainer
    {
        public Trainer()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            Specialization = "";
            ExperienceYears = 0;
            IsActive = true;
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50)]
        public string FirstName { get; set; } 

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50)]
        public string LastName { get; set; }

        //Gostermek amacli veritabanina kaydetmicez
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }


        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        public string Email { get; set; } 

        
        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon giriniz")]
        public string Phone { get; set; } 

        
        [Required(ErrorMessage = "Uzmanlık alanı zorunludur")]
        public string Specialization { get; set; } 

        
        [Range(0, 50, ErrorMessage = "Deneyim 0-50 yıl arası olmalıdır")]
        public int ExperienceYears { get; set; }

        
        public bool IsActive { get; set; } 

        
        public int FitnessCenterId { get; set; }
        public FitnessCenter? FitnessCenter { get; set; }

        
        public ICollection<TrainerAvailability>? Availabilities { get; set; }

        
        public ICollection<Appointment>? Appointments { get; set; }
    }
}