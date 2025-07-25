using System.ComponentModel;

namespace Api_Entregas.ViewModels
{
    public class StudentViewModel
    {
        public int? Id { get; set; }
        [DisplayName("Nome")]
        public string? FirstName { get; set; }
        [DisplayName("Sobrenome")]
        public string? LastName { get; set; }
        [DisplayName("Email")]
        public string? Email { get; set; } = string.Empty;
        [DisplayName("Data de Nascimento")]
        public string? DateOfBirth { get; set; } = string.Empty;
        [DisplayName("Telefone")]
        public string? PhoneNumber { get; set; } = string.Empty;
        [DisplayName("Responsável")]
        public int? ResponsibleId { get; set; }
    }
}