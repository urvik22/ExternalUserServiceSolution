using System.Collections.Generic;
using System.Threading.Tasks;
using ExternalUserService.Models;

namespace ExternalUserService.Services
{
    public interface IExternalUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
