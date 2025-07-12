using System;
using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Enum;

namespace APIBackend.Domain.Identity;

public class ClassDetails
{
    [Key]
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public ClassType ClassType { get; set; }
    public DateTime DateOfClass { get; set; }
    public DateTime DtModified { get; set; } = DateTime.Now;
    public int QuantityHourClass { get; set; }
    public ClassDetails() { }
}