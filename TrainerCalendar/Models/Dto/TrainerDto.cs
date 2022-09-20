using TrainerCalendar.Middlewares;

namespace TrainerCalendar.Models.Dto
{
    public class TrainerDto
    {
        public List<int> Skills { get; set; } = new List<int>();
        public string? TrainerName { get; set; } = null;
        public string? TrainerEmail { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? Password { get; set; } = null;

        public ResponseDto ValidateCreation()
        {
            ResponseDto response = new ResponseDto();
            if (this.TrainerName != null && this.TrainerEmail != null && this.PhoneNumber != null)
            {
                Trainer? t = CurrentRequest.DbContext.Trainers.FirstOrDefault(t => (t.TrainerEmail == this.TrainerEmail || t.PhoneNumber == this.PhoneNumber));
                if (t != null)
                {
                    response.Status = false;
                    response.Message = "Trainer with the email or phone already exist";
                    return response;

                }
                else
                {
                    response.Status = true;
                    response.Message = "You Can Create This Trainer";
                    return response;
                };
            }
            else
            {
                response.Status = false;
                response.Message = "TrainerName, TrainerEmail and PhoneNumber are required in the post request";
                return response;
            };
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
