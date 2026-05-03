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

        public async Task<List<DtoGetBook>> GetAllBooks(string baseUrl)
        {
            var books = await _context.Books.Where(x => x.isDeleted != true).AsNoTracking()
                .Select(x => new DtoGetBook
                {
                    id = x.id,
                    title = x.title,
                    description = x.description ?? "",
                    posterLink = $"{baseUrl}/Media/Images/Books Cover Images/{Path.GetFileName(x.posterPath)}",
                    fileLink = $"{baseUrl}/Media/Books/{Path.GetFileName(x.bookPath)}",
                    categoryId = (int)x.categoryId,
                }).ToListAsync();

            return books;
        }

        public async Task<DtoGetBook> GetBookById(int id,string baseUrl)
        {
            var bookData = await _context.Books.FindAsync(id);
            if (bookData == null)
                return new DtoGetBook();

            var book = new DtoGetBook
            {
                id = bookData.id,
                title = bookData.title,
                description = bookData.description ?? "",
                posterLink = $"{baseUrl}/Media/Images/Books Cover Images/{Path.GetFileName(bookData.posterPath)}",
                fileLink = $"{baseUrl}/Media/Books/{Path.GetFileName(bookData.bookPath)}",
                categoryId = (int)bookData.categoryId,
            };
            return book;
        }
    }
}
