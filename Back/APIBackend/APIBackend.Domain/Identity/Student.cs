using System;
using APIBackend.Domain.Enum;

namespace APIBackend.Domain.Identity;

public class Student
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int? ResponsibleId { get; set; } 
    public User? Responsible { get; set; } = null!;
    public decimal? PriceClasses { get; set; }
    public ICollection<ClassDetails> Classes { get; set; } = new List<ClassDetails>();
}
