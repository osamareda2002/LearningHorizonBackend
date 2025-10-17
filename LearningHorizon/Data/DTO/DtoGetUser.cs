namespace LearningHorizon.Data.DTO
{
    public class DtoGetUser
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool isAdmin { get; set; } = false;
        public string? country { get; set; }
        public string? city { get; set; }
        public string? university { get; set; }
        public string? major { get; set; }
        public string? acadimcYear { get; set; }
        public string? howDidYouKnowUs { get; set; }
        public DateTime? graduationYear { get; set; }

        public List<int> purchasedCourses { get; set; } = new List<int>();

    }
}
