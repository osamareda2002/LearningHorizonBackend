using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<List<DtoGetCourse>> SelectAllCourses();
        Task<DtoGetCourse> SelectCourseById(int id);
    }
}
