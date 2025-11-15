using LearningHorizon.Data;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;

namespace LearningHorizon.Repositories
{
    public class UserExamRepository : GenericRepository<UserExam>, IUserExamRepository
    {
        private readonly ApplicationDbContext _context;
        public UserExamRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
