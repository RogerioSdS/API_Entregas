using System;

namespace APIBackend.Application.DTOs;

public class StudentDTO
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int? ResponsibleId { get; set; } 
}
