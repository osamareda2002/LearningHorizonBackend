using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<string> purchaseCourse(Order order, User user);
    }
}
