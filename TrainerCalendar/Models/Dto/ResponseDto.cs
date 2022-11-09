namespace TrainerCalendar.Models.Dto
{
    public class ResponseDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public ResponseDto() { }

        public ResponseDto(bool status, string message)
        {
            Status = status;
            Message = message;
            Data = new { };
        }
        public ResponseDto(bool status, string message, object data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

     }
}
