using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Entregas.ViewModels
{
    public class UserViewModel
    {
        public string? Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Complement { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Description { get; set; }
    }
}