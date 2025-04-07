using Labb2_Webbutveckling.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labb2_Webbutveckling.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order?> GetOrderByIdAsync(int id);

        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);

        Task<IEnumerable<Order>> SearchOrdersAsync(string query);

        Task AddOrderAsync(Order order);

        Task<bool> DeleteOrderAsync(int id);
    }
}