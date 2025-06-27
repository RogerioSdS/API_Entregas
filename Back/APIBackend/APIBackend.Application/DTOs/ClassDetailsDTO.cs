using System;
using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class ClassDetailsDTO
{
    public required int StudentId { get; set; }
    public required ClassType ClassType { get; set; }
    public required Student Student { get; set; }
    public DateTime DateOfClass { get; set; }
    public required int QuantityHourClass { get; set; }

}
