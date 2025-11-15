using LearningHorizon.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Suggest> Suggests { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserExam> UserExams { get; set; }
        public DbSet<ExamSubmission> ExamSubmissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CoursesShowed)
                .WithMany(c => c.UsersShowed)
                .UsingEntity(j => j.ToTable("UserCoursesShowed"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.CoursesPurchased)
                .WithMany(c => c.UsersPurchased)
                .UsingEntity(j => j.ToTable("UserCoursesPurchased"));

            modelBuilder.Entity<User>()
                .Navigation(u => u.CoursesShowed)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(u => u.CoursesPurchased)
                .AutoInclude();

            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.host)
                .WithMany(u => u.hostedMeetings)
                .HasForeignKey(m => m.hostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.participates)
                .WithMany(u => u.participatedMeetings)
                .UsingEntity(j => j.ToTable("UserMeetings"));

            modelBuilder.Entity<Exam>()
                .HasOne(Exam => Exam.course)
                .WithMany(course => course.Exams)
                .HasForeignKey(Exam => Exam.courseId);

            modelBuilder.Entity<UserExam>()
                .HasOne(ue => ue.user)
                .WithMany(u => u.participatedExams)
                .HasForeignKey(ue => ue.userId);

            modelBuilder.Entity<UserExam>()
                .HasOne(ue => ue.exam)
                .WithMany(e => e.participatedUsers)
                .HasForeignKey(ue => ue.examId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.exam)
                .WithMany(e => e.questions)
                .HasForeignKey(q => q.examId);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.question)
                .WithMany(q => q.answers)
                .HasForeignKey(a => a.questionId);

            modelBuilder.Entity<ExamSubmission>()
                .HasOne(es => es.user)
                .WithMany(u => u.examSubmissions)
                .HasForeignKey(es => es.userId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamSubmission>()
                .HasOne(es => es.exam)
                .WithMany(e => e.examSubmissions)
                .HasForeignKey(es => es.examId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamSubmission>()
                .HasOne(es => es.question)
                .WithMany(q => q.examSubmissions)
                .HasForeignKey(es => es.quesionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamSubmission>()
                .HasOne(es => es.answer)
                .WithMany(a => a.examSubmissions)
                .HasForeignKey(es => es.answerId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}

