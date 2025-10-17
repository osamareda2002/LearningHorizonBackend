using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class Lesson
    {
        [Key]
        [Required]  
        public int id { get; set; }
        [Required]
        public string title { get; set; }
        public string path { get; set; }
        public bool isFree { get; set; }
        public bool isDeleted { get; set; } = false;
        public int? duration { get; set; }

        // Navigation properties
        [Required]
        public int courseId { get; set; }
        public Course course { get; set; }
    }
}
