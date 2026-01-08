using Docosoft.Application.Dtos.Users;
using Docosoft.Application.Extensions;
using Docosoft.Application.Interfaces;
using Docosoft.Domain.Entities;
using Docosoft.Domain.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Docosoft.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ILogger<User> _logger;
        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasher, ILogger<User> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            _logger.LogInformation($"Creating user {dto.Email}");
            User user = dto.ToEntity();
            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
            User userResponse = await _userRepository.AddAsync(user);
            _logger.LogInformation($"User {dto.Email} created successfully!");
            return userResponse.ToDto();

        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            _logger.LogInformation($"Looking for user {id} for deletion");
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User {id} not found for deletion");
                return false;
            }
            _logger.LogInformation($"User {id} found with email {user.Email} for deletion");
            await _userRepository.DeleteAsync(user);
            _logger.LogInformation($"User {user.Email} deleted successfully!");
            return true;
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            _logger.LogInformation($"Fetching all users");
            var users = await _userRepository.GetAllAsync();
            _logger.LogInformation($"Users Found: {users.Count}");
            return users.Select(c => c.ToDto()).ToList();
        }

        public async Task<UserResponseDto?> GetByEmailAsync(string email)
        {
            _logger.LogInformation($"Fething user with email: {email}");
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null) _logger.LogInformation($"User Found: {email}");
            else _logger.LogWarning($"{email} was not found.");
            return user == null ? null : user.ToDto();
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation($"Fething user with id: {id}");
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null) _logger.LogInformation($"User Found: {id}");
            else _logger.LogWarning($"{id} was not found.");
            return user == null ? null : user.ToDto();
        }

        public async Task<List<UserResponseDto>> GetByNameAsync(string name)
        {
            _logger.LogInformation($"Fething users with name: {name}");
            var users = await _userRepository.GetByNameAsync(name);
            _logger.LogInformation($"Users Found: {users.Count} with name {name}");
            return users.Select(c => c.ToDto()).ToList();
        }

        public async Task<List<UserResponseDto>> SearchAsync(string? searchTerm = null, int skip = 0, int take = 0)
        {
            _logger.LogInformation($"Fething users with search therm: {searchTerm} skip:{skip} take: {take}");
            var users = await _userRepository.SearchAsync(searchTerm, skip, take);
            _logger.LogInformation($"{users.Count} Users found with search therm: {searchTerm} skip:{skip} take: {take}");
            return users.Select(c => c.ToDto()).ToList();
        }

        public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            _logger.LogInformation($"Fething user with id: {id} for updating");
            User user = await _userRepository.GetByIdAsync(id);
            if (user == null) {
                _logger.LogWarning($"User with id {id} not found for deletion");
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            if (dto.BirthDate.HasValue)
                user.BirthDate = dto.BirthDate;

            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation($"Updating the user with id: {id}");
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation($"User with id: {id} updated");
            return user.ToDto();

        }
    }
}
