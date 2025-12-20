using FitnessCenterManagement.Models;//modelsteki tablolari db e tanitma 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace FitnessCenterManagement.Data
{
    //kullanici yonetimi ve roller icin identity 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> //ApplicationUser ozellestirilmis user modeli
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //veritabaninda tablolari olusturma
        public DbSet<Models.FitnessCenter> FitnessCenters { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AIRecommendation> AIRecommendations { get; set; }
        
        //tablolar arasi iliskiler icin
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //randevu-antrenor arasi iliski 
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany()
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);//randevuya bagli antrenorun silinmesi engelle
            //randevu-hizmet iliskisi
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
