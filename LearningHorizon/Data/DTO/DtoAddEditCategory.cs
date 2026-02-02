namespace LearningHorizon.Data.DTO
{
    public class DtoAddEditCategory
    {
        public int? id { get; set; }
        public string? title { get; set; }
        public string? about { get; set; }
        public IFormFile? image { get; set; }

    }
}
