namespace LearningHorizon.Data.Models
{
    public class UserCoursesShowed
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int courseId { get; set; }
        public bool isDeleted { get; set; }
        public DateTime deletedDate { get; set; }

        // Navigations
        public virtual User user { get; set; }
        public virtual Course course { get; set; }


    }
}
