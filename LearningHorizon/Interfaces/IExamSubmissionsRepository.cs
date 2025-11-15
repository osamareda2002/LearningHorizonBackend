using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface IExamSubmissionsRepository : IGenericRepository<ExamSubmission>
    {
        Task<DtoReturnExamScore> GetExamResults(DtoGetExamResults dto);
    }
}
