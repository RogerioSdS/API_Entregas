using System.ComponentModel.DataAnnotations;
using APIBackend.Domain.Enum;

public class ClassDetailsDTO
{
    [Required(ErrorMessage = "O ID do estudante é obrigatório.")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "O tipo de aula é obrigatório.")]
    [EnumDataType(typeof(ClassType), ErrorMessage = "Tipo de aula inválido.")]
    public ClassType ClassType { get; set; }
    [Required(ErrorMessage = "A data da aula é obrigatória.")]
    public DateTime DateOfClass { get; set; }
    [Required(ErrorMessage = "A quantidade de horas da aula é obrigatória.")]
    [Range(1, 24, ErrorMessage = "A quantidade de horas deve ser entre 1 e 24.")]
    [Display(Name = "Quantidade de Horas da Aula")]
    public int QuantityHourClass { get; set; }
    public DateTime DtModified { get; set; } = DateTime.Now;
}
