using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;

namespace LearningHorizon.Repositories
{
    public class LessonExerciseRepository : GenericRepository<LessonExercise> , ILessonExerciseRepository
    {
        private readonly ApplicationDbContext _context;
        public LessonExerciseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
