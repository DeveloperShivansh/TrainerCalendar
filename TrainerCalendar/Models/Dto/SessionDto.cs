using TrainerCalendar.Middlewares;

namespace TrainerCalendar.Models.Dto
{
    public class SessionDto
    {
        public string? SessionName { get; set; }
        public string? TrainingMode { get; set; }
        public string? TrainingLocation { get; set; }
        public DateTime StartTime { get; set; } = new DateTime(day: 1, month: 1, year: 1);
        public DateTime EndTime { get; set; } = new DateTime(day: 1, month: 1, year: 1);

        //Navigations
        public int? CourseId { get; set; }

        public int? TrainerId { get; set; }

        public int? SkillId { get; set; }

        public Session? GetSession()
        {
            Session session = new Session();

            session.SessionName = this.SessionName;
            session.CourseId = this.CourseId;
            session.TrainerId = this.TrainerId;
            session.Course = CurrentRequest.DbContext.Courses.FirstOrDefault(s => s.CourseId == this.CourseId);
            session.SkillId = session.Course.SkillId;
            session.StartTime = this.StartTime;
            session.EndTime = this.EndTime;
            session.TrainingLocation = this.TrainingLocation;
            session.TrainingMode = this.TrainingMode;

            return session;
        }
        public void Print()
        {
            Console.WriteLine("<----------SessionDTO------------>");
            Console.WriteLine("SessionName: " + this.SessionName);
            Console.WriteLine("TrainingLocation: " + this.TrainingLocation);
            Console.WriteLine("TrainingMode: " + this.TrainingMode);
            Console.WriteLine("CourseId: " + this.CourseId.ToString());
            Console.WriteLine("TrainerId: " + this.TrainerId.ToString());
            Console.WriteLine("StartTime: " + this.StartTime.ToShortDateString());
            Console.WriteLine("EndTIme: " + this.StartTime.ToShortDateString());
        }
    }
}
