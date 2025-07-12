using System;
using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.DTOs;

public class ClassDetailsUpdateDTO
{
    [Required(ErrorMessage = "O ID da aula é obrigatório.")]
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    [Required(ErrorMessage = "O tipo de aula é obrigatório.")]
    public ClassType ClassType { get; set; }
    public DateTime DateOfClass { get; set; }
    public DateTime DtModified { get; set; } = DateTime.Now;
    /// <summary>
    /// Quantidade de Horas da Aula
    /// </summary>
    [Display(Name = "Quantidade de Horas da Aula")]
    [Required]
    [Range(1, 24, ErrorMessage = "A quantidade de horas deve ser entre 1 e 24.")]
    public int QuantityHourClass { get; set; }
}
