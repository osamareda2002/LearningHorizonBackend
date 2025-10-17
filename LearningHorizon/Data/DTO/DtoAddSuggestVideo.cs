namespace LearningHorizon.Data.DTO
{
    public class DtoAddSuggestVideo
    {
        public string title { get; set; }
        public string instructorName { get; set; }
        public IFormFile file { get; set; }
    }
}
