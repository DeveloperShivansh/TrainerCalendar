using TrainerCalendar.Middlewares;

namespace TrainerCalendar.Models.Dto
{
    public class TrainerDto
    {
        public int TrainerId { get; set; } = -1;
        public List<int> Skills { get; set; } = new List<int>();
        public string? TrainerName { get; set; } = null;
        public string? TrainerEmail { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? Password { get; set; } = null;

        public ResponseDto ValidateCreation()
        {
            Console.WriteLine("Validating data");
            this.Print();
            ResponseDto response = new ResponseDto();
            if (this.TrainerName != null && this.TrainerEmail != null && this.PhoneNumber != null && Skills.Count > 0
                && this.TrainerName.Length >= 4 && this.PhoneNumber.Length > 0)
            {
                Trainer? t = CurrentRequest.DbContext.Trainers.FirstOrDefault(t => (t.TrainerEmail == this.TrainerEmail || t.PhoneNumber == this.PhoneNumber));
                if (t != null)
                {
                    response.Status = false;
                    response.Message = "Trainer with the email or phone already exist, May be you didn't set the password";
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
                response.Message = "TrainerName, TrainerEmail, PhoneNumber and Skills are required ";
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

        public void Print()
        {
            Console.WriteLine("<--------------TrainerDto--------------->");
            Console.WriteLine("TrainerName: ", this.TrainerName);
            Console.WriteLine("TrainerEmail: ", this.TrainerEmail);
            Console.WriteLine("PhoneNumber: ", this.PhoneNumber);
            Console.Write("Skills: ");
            foreach(int id in Skills) { Console.Write(id.ToString() + ", "); }
            Console.WriteLine("");
            Console.WriteLine("<-------------------------------------");
        }
    }
}
