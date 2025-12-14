using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public enum SpecializationType
    {
        [Display(Name = "Fitness")]
        Fitness,

        [Display(Name = "Kas Geliştirme")]
        KasGelistirme,

        [Display(Name = "Kilo Verme")]
        KiloVerme,

        [Display(Name = "Yoga")]
        Yoga,

        [Display(Name = "Pilates")]
        Pilates,

        [Display(Name = "Crossfit")]
        Crossfit
    }
}