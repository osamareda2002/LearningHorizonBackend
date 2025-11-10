namespace LearningHorizon.Data.DTO
{
    public class DtoAddNewMeeting
    {
        public int hostId { get; set; }
        public string? hostEmail { get; set; }
        public string topic { get; set; }
        public DateTime startTime { get; set; }
        public int? durationInMinutes { get; set; }

    }
}
