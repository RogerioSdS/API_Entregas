using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Entregas.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do Email é inválido.")]
        public string Email { get; set; }
        /*[Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "O campo Confirmação de Senha é obrigatório.")]
        [Compare("Password", ErrorMessage = "A confirmação de senha não corresponde à senha.")]
        public string ConfirmPassword { get; set; }*/
    }
}