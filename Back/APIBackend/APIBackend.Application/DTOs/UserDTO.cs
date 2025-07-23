using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class UserDTO
{
  public int Id { get; set; }
  [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. The email must include a valid domain.")]
  public required string Email { get; set; }
  [DataType(DataType.Password)]
  public required string Password { get; set; }
  public required string FirstName { get; set; }
  public string? LastName { get; set; }
  public string Role { get; set; } = "User";
  //propriedade que serão preenchidas somente quando for completar o cadastro do usuário
  public string? Address { get; set; }
  public string? Complement { get; set; }
  public string? PhoneNumber { get; set; }
  public string ZipCode { get; set; } = string.Empty;
  public string? City { get; set; }
  public string? Description { get; set; }
  public bool IsBlocked { get; set; } = false;

  //propriedade que serão preenchidas somente quando o usuário for admin por isso não será usado no DTO
  /* 
  public required bool SignInAfterCreation { get; set; } = false;
  public decimal? CreditLimit { get; set; }
  public bool IsAdmin { get; set; } = false;
  public bool AccessAllowed { get; set; } = false;
  public string? CreditCardNumber { get; set; }
  public int FatureDay { get; set; } = 5;
  */
}
