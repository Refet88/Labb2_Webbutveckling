using Labb2_Webbutveckling.Models;

namespace Labb2_Webbutveckling.Service_Backend
{
    public interface IOrderConfirmationEmailService
    {
        Task SendOrderConfirmationAsync(Order order, Customer customer);
    }
}
