using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<List<DtoGetBook>> GetAllBooks();
        Task<DtoGetBook> GetBookById(int id);
    }
}
