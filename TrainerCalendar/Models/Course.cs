namespace TrainerCalendar.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        //Navigation Properties
        public int SkillId { get; set; }
        public  Skill Skill { get; set; }
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
