using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface ISuggestRepository : IGenericRepository<Suggest>
    {
        Task<List<DtoGetSuggest>> GetAllSuggests();
        Task<DtoGetSuggest> GetById(int id);
    }
}
