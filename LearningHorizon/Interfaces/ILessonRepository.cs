using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        Task<DtoGetLesson> SelectLessonById(int id);
        Task<List<DtoGetLesson>> SelectAllLessons();
        Task<List<DtoGetLesson>> SelectLessonsByCourseId(int courseId);
    }
}
