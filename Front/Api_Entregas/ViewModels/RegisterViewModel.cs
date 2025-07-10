using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Api_Entregas.ViewModels
{
    public class RegisterViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. The email must include a valid domain.")]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; } = "User";
        public string? Address { get; set; }
        public string? Complement { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Description { get; set; }
        public bool IsBlocked { get; set; } = false;
    }
}