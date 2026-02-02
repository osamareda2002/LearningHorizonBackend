using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class CourseCategory
    {
        [Key]
        public int id { get; set; }
        public string? title { get; set; }
        public string? about { get; set; }
        public string? imageUrl { get; set; }
        public DateTime createdTime { get; set; }
        public bool isDeleted { get; set; }

        // Navigation properties
        public virtual ICollection<Course> courses { get; set; } = new HashSet<Course>();
    }
}
