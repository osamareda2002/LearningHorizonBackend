namespace LearningHorizon.Data.DTO
{
    public class DtoAddBook
    {
        public string title { get; set; }
        public string? description { get; set; }
        public IFormFile? coverImage { get; set; }
        public IFormFile bookFile { get; set; }
    }
}
