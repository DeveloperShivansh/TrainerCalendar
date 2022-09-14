namespace TrainerCalendar.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string TrainingMode { get; set; }
        public string TrainingLocation { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        //Navigations
        public int CouresId { get; set; }
        public Course Course { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
