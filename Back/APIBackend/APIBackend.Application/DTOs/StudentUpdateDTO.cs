using System;
using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class UpdateStudentDTO
{
    [Required(ErrorMessage = "O ID do estudante é obrigatório.")]
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal? PriceClasses { get; set; }
    public int? ResponsibleId { get; set; }
}
