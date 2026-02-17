namespace LearningHorizon.Data.DTO
{
    public class DtoAddLesson
    {
        public string title { get; set; }
        public bool isFree { get; set; }
        public int courseId { get; set; }
        public int durationInSeconds { get; set; }
        public int lessonOrder { get; set; }
        public string guid { get; set; }
        public int libraryId { get; set; }
        public IFormFile? lessonFile { get; set; } = null;
        public List<DtoLessonExercise> lessonExercises { get; set; }
    }

    public class DtoLessonExercise
    {
        public string? questionText { get; set; }
        public string? explanation { get; set; }
        public IFormFile? image { get; set; }
        public List<DtoExerciseAnswer> answers { get; set; }
    }


    public class DtoExerciseAnswer
    {
        public string? answerText { get; set; }
        public bool isCorrect { get; set; }
    }
}
