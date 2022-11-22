using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainerCalendar.Models
{
    public class Session
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; }
        [Required(ErrorMessage = "SessionName is required")]
        public string SessionName { get; set; }
        [Required(ErrorMessage = "TrainingMode is required")]
        public string TrainingMode { get; set; }

        [Required(ErrorMessage = "TrainingLocation is required")]
        public string TrainingLocation { get; set; }

        [Required(ErrorMessage = "StartTime is required")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required")]
        public DateTime EndTime { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        //Navigations
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public int? TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public int? SkillId { get; set; }
        public Skill? Skill { get; set; }
    }
}
