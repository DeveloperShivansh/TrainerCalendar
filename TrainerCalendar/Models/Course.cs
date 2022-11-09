using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainerCalendar.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        //Navigation Properties
        public int SkillId { get; set; }
        public  Skill Skill { get; set; }
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
