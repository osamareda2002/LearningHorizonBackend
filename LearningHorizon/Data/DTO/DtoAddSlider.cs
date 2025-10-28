namespace LearningHorizon.Data.DTO
{
    public class DtoAddSlider
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string? link { get; set; }
        public IFormFile file { get; set; }
    }
}
