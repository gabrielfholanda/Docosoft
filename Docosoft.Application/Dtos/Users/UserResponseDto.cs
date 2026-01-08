using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Application.Dtos.Users
{
    public class UserResponseDto
    {
    
        public UserResponseDto(Guid id, string firstName, string lastName, string email,
            string phoneNumber, DateTime? birthDate, bool isActive)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            IsActive = isActive;
        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
