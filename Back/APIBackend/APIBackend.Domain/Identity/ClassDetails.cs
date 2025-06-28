using System;
using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Enum;

namespace APIBackend.Domain.Identity;

public class ClassDetails
{
    [Key]
    public int Id { get; set; }
    public required int StudentId { get; set; }
    public Student? Student { get; set; }
    public required ClassType ClassType { get; set; }
    public required DateTime DateOfClass { get; set; } 
    public DateTime DtModified { get; set; } = DateTime.Now;
    public int QuantityHourClass { get; set; }
}
