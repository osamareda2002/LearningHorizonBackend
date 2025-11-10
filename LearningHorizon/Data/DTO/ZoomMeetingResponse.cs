namespace LearningHorizon.Data.DTO
{
    public class ZoomMeetingResponse
    {
        public long id { get; set; }
        public string? topic { get; set; }
        public DateTime? start_time { get; set; }
        public int duration { get; set; }

        public DateTime created_at { get; set; }
        public string? start_url { get; set; }
        public string? join_url { get; set; }
        public string? password { get; set; }
        public string? h323_password { get; set; }
    }
}
