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
        }



    }
}

