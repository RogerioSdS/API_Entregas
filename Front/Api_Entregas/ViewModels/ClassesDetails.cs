using System;
using Api_Entregas.Services.Enum;

namespace Api_Entregas.ViewModels;

public class ClassesDetails
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public StudentViewModel? Student { get; set; }
    public ClassType ClassType { get; set; }
    public DateTime DateOfClass { get; set; }
    public DateTime DtModified { get; set; } = DateTime.Now;
    public int QuantityHourClass { get; set; }
}
