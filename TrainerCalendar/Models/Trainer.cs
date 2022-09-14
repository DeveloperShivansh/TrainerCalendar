namespace TrainerCalendar.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public string TrainerEmail { get; set; }
        public string PhoneNumber { get; set; }

        //Navigation Properties
        public List<Skill> Skills { get; set; }
        
    }
}
