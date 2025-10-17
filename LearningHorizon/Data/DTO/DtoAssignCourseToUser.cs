namespace LearningHorizon.Data.DTO
{
    public class DtoAssignCourseToUser
    {
        public int userId { get; set; }
        public List<int> courseIds { get; set; }
    }
}
