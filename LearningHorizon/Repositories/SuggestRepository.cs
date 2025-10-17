using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class SuggestRepository : GenericRepository<Suggest> , ISuggestRepository
    {
        private readonly ApplicationDbContext _context;

        public SuggestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetSuggest>> GetAllSuggests()
        {
            var suggests = await _context.Suggests.Where(x => !x.isDeleted).AsNoTracking().Select(x => new DtoGetSuggest
            {
                id = x.id,
                title = x.title,
                path = x.path,
            }).OrderByDescending(x => x.id).ToListAsync();
            return suggests;
        }

        public async Task<DtoGetSuggest> GetById(int id)
        {
            var suggest = await _context.Suggests.Where(x => !x.isDeleted && x.id == id).AsNoTracking()
                .Select(x => new DtoGetSuggest
                {
                    id = x.id,
                    title = x.title,
                    path = x.path,
                }).FirstOrDefaultAsync();
            return suggest != null ? suggest : new DtoGetSuggest();
        }
    }
}
