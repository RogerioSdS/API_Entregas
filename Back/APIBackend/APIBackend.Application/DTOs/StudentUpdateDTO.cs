using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class UpdateStudentDTO
{    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int? ResponsibleId { get; set; } 
    public User? Responsible { get; set; } = null!;
}
