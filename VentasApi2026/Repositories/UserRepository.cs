using Microsoft.EntityFrameworkCore;
using VentasApi2026.Data;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;

namespace VentasApi2026.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) {
            this._context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<List<string>> GetPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToListAsync();

        }

        public async Task<string> GetRoleNameByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .FirstOrDefaultAsync() ?? "Seller";
        }
    }
}
