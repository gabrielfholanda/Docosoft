using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Application.Dtos.Users
{
    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; } = null!;
        [MaxLength(100)]
        public string? LastName { get; set; } = null!;
        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; } = null!;
        [MaxLength(20)]
        public string? PhoneNumber { get; set; } = null!;
        public DateTime? BirthDate { get; set; } = null!;
        public bool? IsActive { get; set; } = null!;
    }
}
