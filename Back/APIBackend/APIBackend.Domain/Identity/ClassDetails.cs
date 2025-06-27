using System;
using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Enum;

namespace APIBackend.Domain.Identity;

public class ClassDetails
{
    [Key]
    public int Id { get; set; }
    public required int StudentId { get; set; }
    public required ClassType ClassType { get; set; }
    public DateTime DateOfClass { get; set; }
    public int QuantityHourClass { get; set; }
}
