using Microsoft.AspNetCore.Identity;

namespace TrainerCalendar.Models
{
    public class User : IdentityUser
    {
        public string Role { get; set; }
        public string Roles { get; set; } = "";

        public bool ValidateRole(string Role)
        {
            if (this.Role == Role) return true;
            string[] roles = this.Roles.Split(',');
            foreach(string role in roles)
            {
                if (role.Equals(Role)) return true;
            }
            return false;
        }
        public List<string> GetRoles()
        {
            string[] roles = this.Roles.Split(',');
            return new List<string>(roles);
        }
    }
}
