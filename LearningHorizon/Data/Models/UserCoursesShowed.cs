namespace LearningHorizon.Data.Models
{
    public class UserCoursesShowed
    {
        public int id { get; set; }
        public int userId { get; set; }
        public User user { get; set; }
        public int courseId { get; set; }
        public Course course { get; set; }
        public bool isDeleted { get; set; }
        public DateTime deletedDate { get; set; }
    }
}
