using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Authentications
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private IConfiguration configuration { get; set; }
        private UserManager<User> userManager { get; set; }
        private SignInManager<User> signInManager { get; set; }

        private ApplicationDbContext dbContext { get; set; }

        public JwtAuthenticationManager(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }
        public User? Authenticate(UserDto userDto)
        {
            string? email = null, phoneNumber = null, password = null;

            email = userDto.Email;
            phoneNumber = userDto.PhoneNumber;
            password = userDto.Password;

            if (email == null && phoneNumber == null) return null;
            else if(password == null) return null;
            else
            {
                User? user = null;
                //searching trainer in User Table Either by phone or email
                if (email != null) user = userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
                else if (phoneNumber != null) user = dbContext.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
                else return null;
                
                //when user not found
                if(user == null) return null;

                //checking for correct password
                var result = userManager.CheckPasswordAsync(user, password).GetAwaiter().GetResult();
                if (result == true) return user;
                else return null;
            }
        }

        public async Task<object> Generate(User user)
        {
            var Key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(Key);
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );
            Console.WriteLine(token.ValidTo.ToString());
            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            };
        }

        public User? Authenticate(TrainerDto trainerDto)
        {
            UserDto userDto = new UserDto();
            userDto.PhoneNumber = trainerDto.PhoneNumber;
            userDto.Email = trainerDto.TrainerEmail;
            userDto.Password = trainerDto.Password;
            return this.Authenticate(userDto);
        }
    }
}
