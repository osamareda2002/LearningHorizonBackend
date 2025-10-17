using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class SliderRepository : GenericRepository<Slider> , ISliderRepository
    {
        private readonly ApplicationDbContext _context;
        public SliderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetSlider>> GetAllSliders()
        {
            var sliders = await _context.Sliders.Where(x => !x.isDeleted).AsNoTracking()
                .Select(s => new DtoGetSlider
                {
                    id = s.id,
                    title = s.title,
                    path = s.path,
                    link = s.link
                }).OrderByDescending(x => x.id).ToListAsync();
            return sliders;
        }
        
        public async Task<DtoGetSlider> GetById(int id)
        {
            var slider = await _context.Sliders.Where(x => !x.isDeleted && x.id == id).AsNoTracking()
                .Select(s => new DtoGetSlider
                {
                    id = s.id,
                    title = s.title,
                    path = s.path,
                    link = s.link
                }).FirstOrDefaultAsync();

            return slider != null ? slider : new DtoGetSlider();
        }
    }
}
