using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class LessonExerciseAnswer
    {
        [Key]
        public int id { get; set; }
        public string answerText { get; set; }
        public bool isCorrect { get; set; } = false;

        [Required]
        public int lessonExerciseId { get; set; }

        // Navigation property
        public virtual LessonExercise lessonExercise { get; set; }
    }
}
