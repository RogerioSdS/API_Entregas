using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APIBackend.Application.DTOs
{
    public class UserUpdateFromUserDTO
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. The email must include a valid domain.")]
        public required string Email { get; set; }
        public string? FirstName { get; set; }       
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }        
        public DateTime Modified { get; set; } = DateTime.Now;
        public string? Complement { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Description { get; set; }
        
    }
}