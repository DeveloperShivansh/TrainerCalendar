using System.Text.RegularExpressions;

namespace TrainerCalendar.Tools
{
    public interface IBasicTools
    {
        public bool IsValidUserName(string username);
    };
    public class BasicTools : IBasicTools
    {
        private string password_regex_pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$";
        private string username_regex_pattern = "^(?=.{5,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$";
        public bool IsValidUserName(string username)
        {
            return Regex.IsMatch(username, username_regex_pattern);
        }

        public bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, password_regex_pattern);
        }

    }
}
