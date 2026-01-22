using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class Exam
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public DateTime startTime { get; set; }
        public int durationInMinutes { get; set; }
        public int courseId { get; set; }
        public bool isDeleted { get; set; }

        // Navigation property
        public virtual Course course { get; set; }
        public virtual ICollection<Question> questions { get; set; } = new HashSet<Question>();
        public virtual ICollection<UserExam> participatedUsers { get; set; } = new HashSet<UserExam>();
        public virtual ICollection<ExamSubmission> examSubmissions { get; set; } = new HashSet<ExamSubmission>();
    }
}
