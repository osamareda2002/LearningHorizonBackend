using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.DTO
{
    public class DtoGetLesson
    {
        public int id { get; set; }
        public string title { get; set; }
        public int arrange { get; set; }
        public string path { get; set; }
        public bool isFree { get; set; }
        public int? duration { get; set; }
        public int? durationInMinutes { get; set; }
        public int courseId { get; set; }
        public string courseTitle { get; set; }
    }
}
