namespace TrainerCalendar.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        //Navigation Properties
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
