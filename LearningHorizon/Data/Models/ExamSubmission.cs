using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class ExamSubmission
    {
        [Key]
        public int id { get; set; }
        public int examId { get; set; }
        public int userId { get; set; }
        public int quesionId { get; set; }
        public int answerId { get; set; }
        public DateTime submissionTime { get; set; }
        public bool isCorrect { get; set; }

        // Navigations
        public virtual User user { get; set; }
        public virtual Exam exam { get; set; }
        public virtual Question question { get; set; }
        public virtual Answer answer { get; set; }

    }
}
