using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningHorizon.Data.Models
{
    public class Meeting
    {
        [Key]
        public int id { get; set; }

        public long meetingId { get; set; }
        public string? topic { get; set; }
        public DateTime? startTime { get; set; }
        public int durationInMinutes { get; set; }

        public int hostId { get; set; }
        public string? hostEmail { get; set; }

        public DateTime createdAt { get; set; }
        public string? startUrl { get; set; }
        public string? joinUrl { get; set; }
        public string? passCode { get; set; }
        public string? numericPassword { get; set; }
        public bool adminJoined { get; set; } = false;

        public bool isFinished { get; set; } = false;
        public bool isDeleted { get; set; }

        // Navigation property

        [ForeignKey(nameof(hostId))]
        public virtual User host { get; set; }
        public virtual ICollection<User> participates { get; set; } = new HashSet<User>();

    }
}
