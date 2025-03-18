using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace APIBackend.Application.DTOs;

public class UserDTO
{
    [Required(ErrorMessage = "The Email field is required.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string  Email { get; set; } 
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Address { get; set; }
    public string? Complement { get; set; }
    public int ZipCode { get; set; } = int.MinValue;
    public string? City { get; set; }
    public string? Description { get; set; }
    public decimal? CreditLimit { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool AccessAllowed { get; set; } = false;
    public string? CreditCardNumber { get; set; }
    public int FatureDay { get; set; } = 5;
    public virtual required ICollection<IdentityUserRole<int>> UserRoles { get; set; }
}
