using LearningHorizon.Data;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;

namespace LearningHorizon.Repositories
{
    public class InstructorRepository : GenericRepository<Instructor> , IInstructorRepository
    {
        private readonly ApplicationDbContext _context;

        public InstructorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        
    }
}
