using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningHorizon.Data.Models
{
    public class Course
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string creator { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        public string path { get; set; }
        public string imagePath { get; set; }
        public bool isDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<User> UsersShowed { get; set; } = new HashSet<User>();
        public virtual ICollection<User> UsersPurchased { get; set; } = new HashSet<User>();

        public virtual ICollection<Lesson> Lessons { get; set; } = new HashSet<Lesson>();
    }
}
