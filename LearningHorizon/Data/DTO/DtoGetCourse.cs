using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.DTO
{
    public class DtoGetCourse
    {
        public int courseId { get; set; }
        public string courseTitle { get; set; }
        public string courseCreator { get; set; }
        public decimal coursePrice { get; set; }
        public string coursePath { get; set; }
        public string courseImagePath { get; set; }
        public int courseDurationInSeconds { get; set; }
        public int lessonsCount { get; set; }
    }
}
