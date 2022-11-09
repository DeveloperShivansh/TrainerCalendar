namespace TrainerCalendar.Models.Dto
{
    public class SessionByDto
    {
        public int SessionId { get; set; } = -1;
        public int TrainerId { get; set; } = -1;
        public int SkillId { get; set; } = -1;
        public int CourseId { get; set; } = -1;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Print()
        {
            Console.WriteLine("<----------- SessionByDto ----------------->");
            Console.WriteLine("SessionId: " + SessionId.ToString());
            Console.WriteLine("TrainerId: " + TrainerId.ToString());
            Console.WriteLine("SkillId: " + SkillId.ToString());
            Console.WriteLine("CourseId: " + CourseId.ToString());
            if(StartTime != null) Console.WriteLine("StartTime: " + StartTime.ToString());
            if(EndTime != null) Console.WriteLine("EndTime: "+ EndTime.ToString());
            Console.WriteLine("----------------------------------------------");
        }
    }
}
