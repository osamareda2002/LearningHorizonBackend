namespace LearningHorizon.Data.DTO
{
    public class DtoAddExercise
    {
        public int lessonId { get; set; }
        public string? questionText { get; set; }
        public string? explanation { get; set; }
        public IFormFile? image { get; set; }
        public List<DtoExerciseAnswer> answers { get; set; }
    }
}
