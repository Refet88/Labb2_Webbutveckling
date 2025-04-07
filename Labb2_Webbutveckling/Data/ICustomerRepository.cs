using Labb2_Webbutveckling.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labb2_Webbutveckling.Data
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        Task<Customer?> GetCustomerByIdAsync(int id);

        Task<Customer?> GetCustomerByEmailAsync(string email);

        Task AddCustomerAsync(Customer customer);

        Task<bool> UpdateCustomerAsync(int id, Customer customer);

        Task DeleteCustomerAsync(int id);
    }
}