using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Authentications
{
    public interface IJwtAuthenticationManager
    {
        public Task<object> Generate(User user);
        public User Authenticate(UserDto userDto);
    }
}
