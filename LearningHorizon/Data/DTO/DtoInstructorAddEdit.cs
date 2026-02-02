namespace LearningHorizon.Data.DTO
{
    public class DtoInstructorAddEdit
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? specialty { get; set; }
        public string? description { get; set; }
        public string? expertise { get; set; }
        public IFormFile? image { get; set; }
        public string? facebookUrl { get; set; }
        public string? whatsappUrl { get; set; }
        public string? instgramUrl { get; set; }
    }
}
