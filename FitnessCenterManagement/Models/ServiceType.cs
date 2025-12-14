using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models
{
    public enum ServiceType
    {
        [Display(Name = "Genel Fitness")]
        GenelFitness,

        [Display(Name = "Functional Training")]
        FunctionalTraining,

        [Display(Name = "HIIT (Yüksek Yoğunluklu Antrenman)")]
        HIIT,

        [Display(Name = "Bodybuilding")]
        Bodybuilding,

        [Display(Name = "Powerlifting")]
        Powerlifting,

        [Display(Name = "Crossfit")]
        Crossfit,

        [Display(Name = "Kardiyo")]
        Kardiyo,

        [Display(Name = "Spinning")]
        Spinning,

        [Display(Name = "Zumba")]
        Zumba,

        [Display(Name = "Yoga")]
        Yoga,

        [Display(Name = "Pilates")]
        Pilates,

        [Display(Name = "Stretching")]
        Stretching,

        [Display(Name = "Kişisel Antrenman")]
        KisiselAntrenman,

        [Display(Name = "Grup Dersleri")]
        GrupDersleri,

        [Display(Name = "Beslenme Danışmanlığı")]
        BeslenmeDanismanligi
    }
}
