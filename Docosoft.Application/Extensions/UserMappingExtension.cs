using Docosoft.Application.Dtos.Users;
using Docosoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Application.Extensions
{
    public static class UserMappingExtension
    {
        public static User ToEntity(this CreateUserDto dto)
        {
            if (dto == null) return null;
            return new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                PasswordHash = dto.Password,
                IsActive = true,
                CreatedAt = DateTimeOffset.Now,
                PhoneNumber = dto.PhoneNumber,
            };

        }

        public static User ToEntity(this UpdateUserDto dto)
        {
            if (dto == null) return null;
            return new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                IsActive = dto.IsActive.HasValue ? dto.IsActive.Value : true,
            };

        }

        public static UserResponseDto ToDto(this User entity)
        {
            return new UserResponseDto(id: entity.Id, firstName: entity.FirstName, lastName: entity.LastName, email: entity.Email,
                phoneNumber: entity.PhoneNumber, birthDate: entity.BirthDate, isActive: entity.IsActive);
        }
    }
}
