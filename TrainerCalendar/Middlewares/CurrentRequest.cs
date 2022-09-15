using Microsoft.AspNetCore.Identity;
using TrainerCalendar.Authentications;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models;

namespace TrainerCalendar.Middlewares
{
    public class CurrentUser {
        public string UserName { get; set; }   
        public string Email { get; set; }  
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public User User { get; set; }
        public Trainer? GetTrainer(ApplicationDbContext dbContext)
        {
            if (Role == "Trainer") return dbContext.Trainers.FirstOrDefault(t => t.TrainerEmail == Email);
            else return null;
        }

        public bool IsTrainer()
        {
            if (Role == "Trainer") return true;
            else return false;
        }
    }
    public class CurrentRequest
    {
        public static IConfiguration Configuration { get; set; }
        public static ApplicationDbContext DbContext { get; set; } 
        public static UserManager<User> UserManager { get; set; }
        public static IJwtAuthenticationManager JwtAuthenticationManager { get; set; }
        public static CurrentUser? CurrentUser { get; set; }
    }
}
