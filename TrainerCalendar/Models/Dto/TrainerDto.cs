namespace TrainerCalendar.Models.Dto
{
    public class TrainerDto
    {
        public string? TrainerName { get; set; } = null;
        public string? TrainerEmail { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? Password { get; set; } = null;

        public bool ValidateCreation()
        {
            if (this.TrainerName != null && this.TrainerEmail != null && this.PhoneNumber != null && this.Password != null)
            {
                return true;
            }
            else return false;
        }

        public bool ValidateForSettingPassword()
        {
            if ((this.TrainerEmail != null || this.PhoneNumber != null) && this.Password != null)
            {
                return true;
            }
            else return false;
        }


    }
}
