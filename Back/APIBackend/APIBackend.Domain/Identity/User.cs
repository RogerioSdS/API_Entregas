using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace APIBackend.Domain.Identity;

public class User : IdentityUser<int>
{
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public override string? Email { get; set; } // Sobrescreve a propriedade da classe base
    [NotMapped]// Ignora no banco de dados
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Complement { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Description { get; set; }
    [NotMapped]
    public required string Role { get; set; }
    public decimal AgreedPrice { get; set; }
    public bool SignInAfterCreation { get; set; } = false;
    public decimal? CreditLimit { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool AccessAllowed { get; set; } = false;
    public string? CreditCardNumber { get; set; }
    public int FatureDay { get; set; } = 5;
    public IEnumerable<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public IEnumerable<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();
}