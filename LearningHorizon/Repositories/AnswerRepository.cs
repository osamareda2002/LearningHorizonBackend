using LearningHorizon.Data;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;

namespace LearningHorizon.Repositories
{
    public class AnswerRepository : GenericRepository<Answer> , IAnswerRepository
    {
        private readonly ApplicationDbContext _context;
        public AnswerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
