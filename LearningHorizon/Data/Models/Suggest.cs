using Microsoft.AspNetCore.Components.Web;

namespace LearningHorizon.Data.Models
{
    public class Suggest
    {
        public int id { get; set; }
        public string title { get; set; }
        public string path { get; set; }
        public string instructorName { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
