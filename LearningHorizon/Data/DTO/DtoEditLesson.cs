namespace LearningHorizon.Data.DTO
{
    public class DtoEditLesson
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool isFree { get; set; }
        public int courseId { get; set; }
        public IFormFile lessonFile { get; set; }
    }
}
