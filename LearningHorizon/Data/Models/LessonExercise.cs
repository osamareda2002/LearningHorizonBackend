using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class LessonExercise
    {
        [Key]
        public int id { get; set; }
        public int lessonId { get; set; }
        [Required]
        public string questionText { get; set; }
        public string? imageLink { get; set; }
        public string explanation { get; set; }

        // Navigation property
        public virtual Lesson lesson { get; set; }
        public virtual ICollection<LessonExerciseAnswer> lessonExerciseAnswers { get; set; } = new HashSet<LessonExerciseAnswer>();
    }
}
