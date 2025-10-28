using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<DtoGetUser>> SelectAllUsers();
        Task<DtoGetUser> GetUserById(int id);
        Task<DtoGetUser> EditUser(DtoUpdateUser dtoUser);
        Task AddPurchasedCourse(int courseId, int userId);
        Task<List<User>> GetAllUsersIncluding();
    }
}
