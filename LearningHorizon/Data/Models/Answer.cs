using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class Answer
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string answerText { get; set; }
        public bool isCorrect { get; set; }
        public int questionId { get; set; }


        // Navigation property
        public virtual Question question { get; set; }
        public virtual ICollection<ExamSubmission> examSubmissions { get; set; } = new HashSet<ExamSubmission>();

    }
}
