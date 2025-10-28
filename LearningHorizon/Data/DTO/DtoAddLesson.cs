namespace LearningHorizon.Data.DTO
{
    public class DtoAddLesson
    {
        public string title { get; set; }
        public bool isFree { get; set; }
        public int courseId { get; set; }
        public int durationInSeconds { get; set; }
        public int lessonOrder { get; set; }
        public IFormFile lessonFile { get; set; }
    }
}
