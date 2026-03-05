using VentasApi2026.Models;

namespace VentasApi2026.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<List<string>> GetPermissionsAsync(int userId);
        Task<string> GetRoleNameByUserIdAsync(int userId);
    }
}
