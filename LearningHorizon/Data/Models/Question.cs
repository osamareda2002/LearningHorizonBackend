using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class Question
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string questionText { get; set; }
        public double mark { get; set; }
        public int examId { get; set; }


        // Navigation property
        public virtual Exam exam { get; set; }
        public virtual ICollection<Answer> answers { get; set; } = new HashSet<Answer>();
        public virtual ICollection<ExamSubmission> examSubmissions { get; set; } = new HashSet<ExamSubmission>();

    }
}
