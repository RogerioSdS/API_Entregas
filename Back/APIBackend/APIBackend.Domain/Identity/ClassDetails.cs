using System;
using APIBackend.Domain.Enum;

namespace APIBackend.Domain.Identity;

public class ClassDetails
{
    public Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public required ClassType ClassType { get; set; }
    public required Student Student { get; set; }
    public DateTime DateOfClass { get; set; }
}
