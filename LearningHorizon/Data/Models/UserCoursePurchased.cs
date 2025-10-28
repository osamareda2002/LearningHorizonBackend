namespace LearningHorizon.Data.Models
{
    public class UserCoursePurchased
    {
        public int id { get; set; }
        public int userId { get; set; }
        public User user { get; set; }

        public int courseId { get; set; }
        public Course course { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
        public bool isDeleted { get; set; }
    }

}
