namespace LearningHorizon.Data.DTO
{
    public class DtoAddCourse
    {
        public int? courseId { get; set; }
        public string courseTitle { get; set; }
        public string courseCreator { get; set; }
        public decimal? coursePrice { get; set; }
        public IFormFile? courseImage { get; set; }
    }
}
