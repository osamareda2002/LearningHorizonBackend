using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace LearningHorizon.Data.Models
{
    public class User
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        public bool isDeleted { get; set; } = false;
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        public string? country { get; set; }
        public string? city { get; set; }
        public string? university { get; set; }
        public string? major { get; set; }
        public string? acadimcYear { get; set; }
        public string? howDidYouKnowUs { get; set; }
        public DateTime? graduationYear { get; set; }
        public string? profilePic { get; set; }
        public string? resetToken { get; set; }
        public DateTime? resetTokenExpiry { get; set; }


        public bool isAdmin { get; set; } = false;
        public bool isOwner { get; set; } = false;
        public string? lastToken { get; set; }


        // Navigation properties
        public virtual ICollection<Course> CoursesShowed { get; set; } = new HashSet<Course>();
        public virtual ICollection<Course> CoursesPurchased { get; set; } = new HashSet<Course>();

    }
}
