using Microsoft.AspNetCore.Identity;

namespace FitnessCenterManagement.Models
{
    //Ad-soyad ikilisini kaydolurken kullanmak icin özelelstirilmis 
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
