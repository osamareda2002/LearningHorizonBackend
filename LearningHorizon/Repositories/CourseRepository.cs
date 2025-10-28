using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class CourseRepository : GenericRepository<Course> , ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetCourse>> SelectAllCourses(string baseUrl)
        {
            var courses = await _context.Courses.Where(x => !x.isDeleted).AsNoTracking()
                .Select(c => new DtoGetCourse
                {
                    courseId = c.id,
                    courseTitle = c.title,
                    courseCreator = c.creator,
                    coursePrice = c.price,
                    coursePath = c.path,
                    courseImagePath = $"{baseUrl}/Media/Images/CourseImages/{Path.GetFileName(c.imagePath)}",
                    lessonsCount = c.Lessons.Count,
                    courseDurationInSeconds = c.Lessons.Sum(l => l.duration ?? 0)
                }).OrderByDescending(x => x.courseId).ToListAsync();
            return courses;
        }
        public async Task<DtoGetCourse> SelectCourseById(int id)
        {
            var course = await _context.Courses.AsNoTracking()
                .Where(c => c.id == id && !c.isDeleted)
                .Select(c => new DtoGetCourse
                {
                    courseId = c.id,
                    courseTitle = c.title,
                    courseCreator = c.creator,
                    coursePrice = c.price,
                    coursePath = c.path,
                    courseImagePath = c.imagePath,
                    courseDurationInSeconds = c.Lessons.Sum(l => l.duration ?? 0)
                }).FirstOrDefaultAsync();
            return course;
        }

        
    }
}
