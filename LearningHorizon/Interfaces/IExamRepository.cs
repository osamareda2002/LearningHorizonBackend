using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        Task<List<DtoGetExam>> GetUpcomingExams(int userId);
        Task<List<DtoGetExam>> GetAllExams();
        Task<DtoGetExam> AddExam(DtoAddExam dtoExam);
        Task<DtoGetExamQuestions> GetExamQuestions(int examId);

    }
}
