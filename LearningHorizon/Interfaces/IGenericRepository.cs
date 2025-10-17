using System.Linq.Expressions;

namespace LearningHorizon.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<ICollection<T>> GetAllAsync();
        Task SaveChangesAsync();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        List<T> FindBy(Expression<Func<T, bool>> predicate);
        string GetContentType(string path);


    }
}
