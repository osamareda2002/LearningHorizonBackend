using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface ICourseCategoryRepository : IGenericRepository<CourseCategory>
    {
        Task<List<DtoGetCategory>> GetAllCategories(string baseUrl);
        Task<List<DtoGetCategory>> AddCategory(DtoAddEditCategory dto, string baseUrl);
        Task<List<DtoGetCategory>> EditCategory(DtoAddEditCategory dto, string baseUrl);
        Task<List<DtoGetCategory>> DeleteCategory(int id, string baseUrl);
    }
}
