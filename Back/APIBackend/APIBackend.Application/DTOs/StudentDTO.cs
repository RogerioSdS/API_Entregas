using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class StudentDTO
{
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<int>? ResponsibleId { get; set; } 
    public List<User>? Responsibles { get; set; }
}
