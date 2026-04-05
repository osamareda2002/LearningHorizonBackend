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
        public List<DtoGetLessonExercise> mcq { get; set; }
    }

    public class DtoGetLessonExercise
    {
        public int id { get; set; }
        public string? questionText { get; set; }
        public string? explanation { get; set; }
        public string? quoteSubject { get; set; } = string.Empty;
        public string? quoteBody { get; set; } = string.Empty;
        public string? imageLink { get; set; }
        public List<DtoGetExerciseAnswer> answers { get; set; }
    }


    public class DtoGetExerciseAnswer
    {
        public int id { get; set; }
        public string? answerText { get; set; }
        public bool isCorrect { get; set; }
    }
}
