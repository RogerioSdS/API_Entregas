using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace Api_Entregas.ViewModels
{
    public class RegisterViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. The email must include a valid domain.")]
        public required string Email { get; set; }
        [DisplayName("Senha")]
        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DisplayName("Confirme a senha")]
        [Required(ErrorMessage = "Confirme a senha")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public required string ConfirmPassword { get; set; }

        [DisplayName("Primeiro Nome")]
        public required string FirstName { get; set; }

        [DisplayName("Sobrenome")]
        public string? LastName { get; set; }

        [DisplayName("Telefone")]
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "User";

        [DisplayName("Endereço")]
        public string? Address { get; set; }

        [DisplayName("Complemento")]
        public string? Complement { get; set; }

        [DisplayName("CEP")]
        public string? ZipCode { get; set; }

        [DisplayName("Cidade")]
        public string? City { get; set; }
        public string? Description { get; set; }
        public bool IsBlocked { get; set; } = false;
    }
}