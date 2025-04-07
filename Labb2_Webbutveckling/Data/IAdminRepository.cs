using Labb2_Webbutveckling.Models;

namespace Labb2_Webbutveckling.Data
{
    public interface IAdminRepository 
    {
        Task<Admin?> GetAdminByUsernameAsync(string username);
        Task AddAdminAsync(Admin admin);
    }
}
