using System;
using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class ClassDetailsUpdateDTO
{    
    public required int Id { get; set; }
    public int? StudentId { get; set; }
    public ClassType? ClassType { get; set; }
    public Student? Student { get; set; }
    public DateTime? DateOfClass { get; set; }
    public DateTime? DtModified { get; set; }
    public int? QuantityHourClass { get; set; }
}
