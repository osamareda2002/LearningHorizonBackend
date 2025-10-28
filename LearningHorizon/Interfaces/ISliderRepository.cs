using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface ISliderRepository : IGenericRepository<Slider>
    {
        Task<List<DtoGetSlider>> GetAllSliders(string baseUrl);
        Task<DtoGetSlider> GetById(int id, string baseUrl);
    }
}
