using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Entregas.ViewModels
{
    public class SignInViewModel
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public string? Email { get; set; }
        public DateTime StartSession { get; set; } = DateTime.Now;
        public bool SignIn { get; set; }
    }
}