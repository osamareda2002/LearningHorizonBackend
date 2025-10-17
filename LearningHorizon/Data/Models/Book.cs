using System.ComponentModel.DataAnnotations;

namespace LearningHorizon.Data.Models
{
    public class Book
    {
        [Key]
        [Required]
        public int id { get; set; }

        [Required]
        public string title { get; set; }

        public string? posterPath { get; set; }
        public string bookPath { get; set; }
        public string? description { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? deletedAt { get; set; }
    }
}
