using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend_Entregas.Models
{
    public class Users
    {
        [Required]
        [DataType(DataType.EmailAddress)]      
        public required string Email { get; set; }
         [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]).{8,}$",
            ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, um número e um caractere especial.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 8)]
        public required string Password { get; set; }

    }
}