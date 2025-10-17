using Azure;
using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class BookRepository : GenericRepository<Book> , IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetBook>> GetAllBooks()
        {
            var books = await _context.Books.Where(x => x.isDeleted != true).AsNoTracking()
                .Select(x => new DtoGetBook
                {
                    id = x.id,
                    title = x.title,
                    description = x.description ?? "",
                    posterLink = x.posterPath
                }).ToListAsync();

            return books;
        }

        public async Task<DtoGetBook> GetBookById(int id)
        {
            var bookData = await _context.Books.FindAsync(id);
            if (bookData == null)
                return new DtoGetBook();

            var book = new DtoGetBook
            {
                id = bookData.id,
                title = bookData.title,
                description = bookData.description ?? "",
            };
            return book;
        }
    }
}
