using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LearningHorizon.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        private readonly ApplicationDbContext _context;

        public LessonRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DtoGetLesson> SelectLessonById(int id)
        {
            var lesson = await _context.Lessons.AsNoTracking()
                .Where(l => l.id == id && !l.isDeleted)
                .Select(l => new DtoGetLesson
                {
                    id = l.id,
                    title = l.title,
                    path = l.path,
                    isFree = l.isFree,
                    courseId = l.courseId,
                    duration = l.duration ?? 0
                }).FirstOrDefaultAsync();
            return lesson;
        }
        public async Task<List<DtoGetLesson>> SelectAllLessons()
        {
            var lessons = await _context.Lessons.Where(x => !x.isDeleted).AsNoTracking()
                .Select(l => new DtoGetLesson
                {
                    id = l.id,
                    title = l.title,
                    path = l.path,
                    isFree = l.isFree,
                    courseId = l.courseId,
                    duration = l.duration ?? 0
                }).ToListAsync();
            return lessons;
        }
        public async Task<List<DtoGetLesson>> SelectLessonsByCourseId(int courseId)
        {
            var lessons = await _context.Lessons.Where(x => x.courseId == courseId && !x.isDeleted).AsNoTracking()
                .Select(l => new DtoGetLesson
                {
                    id = l.id,
                    title = l.title,
                    path = l.path,
                    isFree = l.isFree,
                    courseId = l.courseId,
                    duration = l.duration ?? 0
                }).ToListAsync();
            return lessons;
        }
    }
}
