using Docosoft.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Application.Interfaces
{
    public interface IUserService
    {
       
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);

        Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);

        Task<bool> DeleteUserAsync(Guid id);

        Task<UserResponseDto?> GetByIdAsync(Guid id);

        Task<UserResponseDto?> GetByEmailAsync(string email);

        Task<List<UserResponseDto>> GetByNameAsync(string name);

        Task<List<UserResponseDto>> GetAllAsync();

        Task<List<UserResponseDto>> SearchAsync(string? searchTerm = null, int skip = 0, int take = 0);
    }
}
