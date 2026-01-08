using Docosoft.Domain.Entities;
using Docosoft.Domain.Repositories;
using Docosoft.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DocosoftDbContext _dbContext;

        public UserRepository(DocosoftDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email.Contains(email));
        }
        public async Task<List<User>> GetByNameAsync(string name)
        {

            return await _dbContext.Users
                .Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name))
                .ToListAsync();
        }

        public async Task<List<User>> SearchAsync(string? searchTerm = null, int skip = 0, int take = 0)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();

                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(searchTerm))

                );
            }

            query = query.OrderBy(u => u.FirstName);
            if (take > 0)
                query = query.Skip(skip).Take(take);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
           return await _dbContext.Users.AsNoTracking().ToListAsync();
        }
    }
}
