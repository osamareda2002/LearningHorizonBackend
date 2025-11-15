namespace LearningHorizon.Data.DTO
{
    public class DtoGetExam
    {
        public int? id { get; set; }
        public string title { get; set; }
        public DateTime startTime { get; set; }
        public int durationInMinutes { get; set; }
        public int courseId { get; set; }
        public string? courseName { get; set; }
        public int? currentQuestionId { get; set; }

    }

    public class DtoAddExam
    {
        public string examTitle { get; set; }
        public DateTime startTime { get; set; }
        public int duration { get; set; }
        public int? courseId { get; set; }

    }
}
