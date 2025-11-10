namespace LearningHorizon.Data.DTO
{
    public class DtoGetMeetingInfo
    {
        public int id { get; set; }
        public long meetingId { get; set; }
        public string? topic { get; set; }
        public DateTime? startTime { get; set; }
        public int durationInMinutes { get; set; }
        public string? hostName { get; set; }
        public string? startUrl { get; set; }
        public string? joinUrl { get; set; }
        public DateTime createdAt { get; set; }
        public bool adminJoined { get; set; }
        public bool isFinished { get; set; }
    }
}
