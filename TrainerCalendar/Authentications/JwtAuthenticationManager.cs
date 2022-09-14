using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Authentications
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        public User Authenticate(UserDto userDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> Generate(User user)
        {
            throw new NotImplementedException();
        }
    }
}
