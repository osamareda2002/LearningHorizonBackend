using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class UserExam
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public int examId { get; set; }
        public int? currentQuestionId { get; set; }
        public bool userFinished { get; set; } = false;
        // Navigations
        public virtual User user { get; set; }
        public virtual Exam exam { get; set; }
    }
}
