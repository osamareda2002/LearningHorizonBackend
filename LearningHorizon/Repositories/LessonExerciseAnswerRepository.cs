using LearningHorizon.Data;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;

namespace LearningHorizon.Repositories
{
    public class LessonExerciseAnswerRepository : GenericRepository<LessonExerciseAnswer> , ILessonExerciseAnswerRepository
    {
        private readonly ApplicationDbContext _context;
        public LessonExerciseAnswerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }   
    }
}
