namespace LearningHorizon.Data.Models
{
    public class Order
    {
        public int id { get; set; }
        public string paymobOrderId { get; set; }
        public int userId { get; set; }
        public int courseId { get; set; }
        public decimal totalAmount { get; set; }
        public string? status { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;



        // Navigation property
        public User user { get; set; }
        public Course course { get; set; }
    }
}
