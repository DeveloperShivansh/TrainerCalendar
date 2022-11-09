using System.ComponentModel.DataAnnotations;

namespace TrainerCalendar.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Trainer Name Is Required")]
        public string TrainerName { get; set; }
        [Required(ErrorMessage = "TrainerEmail Is Required")]
        [EmailAddress]
        public string TrainerEmail { get; set; }
        [Required(ErrorMessage = "Trainer Phone Number Is Required")]
        [Phone]
        public string PhoneNumber { get; set; }

        //Navigation Properties
        public User? User { get; set; }
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
