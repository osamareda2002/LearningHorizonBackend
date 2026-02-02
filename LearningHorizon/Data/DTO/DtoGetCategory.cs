namespace LearningHorizon.Data.DTO
{
    public class DtoGetCategory
    {
        public int id { get; set; }
        public string title { get; set; }
        public string? about { get; set; }
        public string? imageUrl { get; set; }
        public List<DtoGetCourse> courses { get; set; }
    }
}
