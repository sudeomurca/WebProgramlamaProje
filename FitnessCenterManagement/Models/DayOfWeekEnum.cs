using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public enum DayOfWeekEnum
    {
        [Display(Name = "Pazartesi")]
        Monday,

        [Display(Name = "Salı")]
        Tuesday,

        [Display(Name = "Çarşamba")]
        Wednesday,

        [Display(Name = "Perşembe")]
        Thursday,

        [Display(Name = "Cuma")]
        Friday,

        [Display(Name = "Cumartesi")]
        Saturday,

        [Display(Name = "Pazar")]
        Sunday
    }
}