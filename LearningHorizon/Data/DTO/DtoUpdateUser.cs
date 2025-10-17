namespace LearningHorizon.Data.DTO
{
    public class DtoUpdateUser
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string university { get; set; }
        public string major { get; set; }
        public string acadimcYear { get; set; }
        public string howDidYouKnowUs { get; set; }
        public DateTime graduationYear { get; set; }
    }
}
