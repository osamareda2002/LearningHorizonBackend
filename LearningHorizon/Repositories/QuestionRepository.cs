using LearningHorizon.Data;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;

namespace LearningHorizon.Repositories
{
    public class QuestionRepository : GenericRepository<Question> , IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
