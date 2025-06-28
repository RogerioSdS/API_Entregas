using System;
using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class ClassDetailsDTO
{
    public int StudentId { get; set; }
    public required ClassType ClassType { get; set; }
    public Student? Student { get; set; }
    public required DateTime DateOfClass { get; set; } 
    public DateTime DtModified { get; set; } = DateTime.Now;
    public required int QuantityHourClass { get; set; }
}
