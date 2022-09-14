namespace TrainerCalendar.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }

        //Navigation Properties
        public List<Course> Courses { get; set; }  = new List<Course>();
        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<Trainer> Trainers { get; set; } = new List<Trainer>();
    }
}
