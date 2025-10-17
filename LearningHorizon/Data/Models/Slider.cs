using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class Slider
    {
        [Key]
        public int id { get; set; }
        public string? title { get; set; }
        public string? path { get; set; }
        public string? link { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
