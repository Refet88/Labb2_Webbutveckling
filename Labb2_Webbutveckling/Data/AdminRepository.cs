using Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Labb2_Webbutveckling.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ECommerceDbContext _context;

        public AdminRepository(ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<Admin?> GetAdminByUsernameAsync(string username)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
        }


        public async Task AddAdminAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }
    }
}