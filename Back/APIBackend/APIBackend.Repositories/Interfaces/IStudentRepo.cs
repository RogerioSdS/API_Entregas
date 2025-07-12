using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

/// <summary>
/// Interface responsável pelas operações de repositório relacionadas aos estudantes.
/// </summary>
public interface IStudentRepo
{
    /// <summary>
    /// Adiciona um novo estudante ao banco de dados.
    /// </summary>
    /// <param name="student">O estudante a ser adicionado.</param>
    /// <returns>O estudante criado.</returns>
    Task<Student> AddStudentAsync(Student student);

    /// <summary>
    /// Retorna todos os estudantes cadastrados.
    /// </summary>
    /// <returns>Lista de estudantes ou null se nenhum for encontrado.</returns>
    Task<List<Student>?> GetStudentAsync();

    /// <summary>
    /// Retorna um estudante pelo seu ID.
    /// </summary>
    /// <param name="id">ID do estudante.</param>
    /// <returns>O estudante correspondente ou null se não encontrado.</returns>
    Task<Student>? GetStudentByIdAsync(int? id);

    /// <summary>
    /// Retorna uma lista de estudantes com o nome especificado.
    /// </summary>
    /// <param name="name">Nome do estudante.</param>
    /// <returns>Lista de estudantes com o nome informado ou null se nenhum for encontrado.</returns>
    Task<List<Student>?> GetStudentByNameAsync(string name);

    /// <summary>
    /// Atualiza os dados de um estudante.
    /// </summary>
    /// <param name="student">Estudante com dados atualizados.</param>
    /// <returns>Estudante atualizado.</returns>
    Task<Student> UpdateStudentAsync(Student student);

    /// <summary>
    /// Remove um estudante com base em seu ID.
    /// </summary>
    /// <param name="id">ID do estudante a ser removido.</param>
    /// <returns>True se a exclusão foi bem-sucedida, caso contrário false.</returns>
    Task<bool> DeleteStudentAsync(int? id);
}
