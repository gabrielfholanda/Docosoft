using Docosoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetByNameAsync(string name);

        Task<List<User>> GetAllAsync();

        Task<List<User>> SearchAsync(
            string? searchTerm = null,
            int skip = 0,
            int take = 0);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
