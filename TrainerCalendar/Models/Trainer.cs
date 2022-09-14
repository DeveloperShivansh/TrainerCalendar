namespace TrainerCalendar.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }

        //Navigation Properties
        public List<Skill> Skills { get; set; }
        
    }
}
