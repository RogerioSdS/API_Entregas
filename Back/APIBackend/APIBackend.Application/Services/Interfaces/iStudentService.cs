using System;
using APIBackend.Application.DTOs;

namespace APIBackend.Application.Services.Interfaces;

public interface IStudentService
{
    /// <summary>
    /// Adiciona um novo estudante ao sistema.
    /// </summary>
    /// <param name="studentDTO">DTO com os dados do estudante a ser adicionado.</param>
    /// <returns>Objeto <see cref="StudentDTO"/> representando o estudante adicionado.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o <paramref name="studentDTO"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando ocorre erro ao adicionar o estudante.</exception>
    Task<StudentDTO> AddStudentAsync(StudentDTO studentDTO);

    /// <summary>
    /// Retorna todos os estudantes cadastrados.
    /// </summary>
    /// <returns>Lista de objetos <see cref="StudentDTO"/>.</returns>
    Task<List<StudentDTO>> GetStudentsAsync();

    /// <summary>
    /// Busca um estudante pelo ID.
    /// </summary>
    /// <param name="id">ID do estudante.</param>
    /// <returns>Objeto <see cref="StudentDTO"/> correspondente.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Lançada quando o ID é inválido.</exception>
    /// <exception cref="NullReferenceException">Lançada quando o estudante não é encontrado.</exception>
    Task<StudentDTO> GetStudentByIdAsync(int id);

    /// <summary>
    /// Busca estudantes pelo nome.
    /// </summary>
    /// <param name="name">Nome do estudante.</param>
    /// <returns>Lista de <see cref="StudentDTO"/> com correspondência.</returns>
    /// <exception cref="ArgumentException">Lançada quando o nome é nulo ou vazio.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando nenhum estudante é encontrado.</exception>
    Task<List<StudentDTO>> GetStudentByNameAsync(string name);

    /// <summary>
    /// Atualiza as informações de um estudante.
    /// </summary>
    /// <param name="studentDTO">DTO com os dados atualizados do estudante.</param>
    /// <returns>Objeto <see cref="UpdateStudentDTO"/> com os dados atualizados.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o <paramref name="studentDTO"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando ocorre erro ao atualizar.</exception>
    Task<UpdateStudentDTO> UpdateStudentAsync(UpdateStudentDTO studentDTO);

    /// <summary>
    /// Exclui um estudante pelo ID.
    /// </summary>
    /// <param name="id">ID do estudante.</param>
    /// <returns>Verdadeiro se a exclusão for bem-sucedida.</returns>
    /// <exception cref="NullReferenceException">Lançada quando o estudante não é encontrado.</exception>
    Task<bool> DeleteStudentAsync(int id);
}
