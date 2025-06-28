using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces
{
    public interface IClassDetailsRepo
    {
        /// <summary>
        /// Adiciona uma nova aula no banco de dados.
        /// </summary>
        /// <param name="classDetails">Entidade contendo os dados da aula a ser criada.</param>
        /// <returns>A aula criada.</returns>
        public Task<ClassDetails> AddClassDetailsAsync(ClassDetails classDetails);
                
        /// <summary>
        /// Busca todas as aulas ordenadas por data.
        /// </summary>
        /// <returns>Uma lista contendo todas as aulas.</returns>
        public Task<IEnumerable<ClassDetails>> GetAllClassesByDateAsync(string? dateFrom, string? dateTo, int? studentId = null);

        /// <summary>
        /// Busca uma aula pelo seu ID.
        /// </summary>
        /// <param name="classId">ID da aula.</param>
        /// <returns>A aula correspondente ao ID informado, ou <see langword="null"/> se não encontrada.</returns>
        public Task<ClassDetails?> GetClassDetailsByIdAsync(int classId);

        /// <summary>
        /// Busca uma aula associada a um aluno específico.
        /// </summary>
        /// <param name="studentId">ID do aluno.</param>
        /// <returns>A aula associada ao aluno, ou <see langword="null"/> se não encontrada.</returns>
        public Task<ClassDetails?> GetClassesDetailsStudentIdAsync(int studentId);

        /// <summary>
        /// Atualiza os dados de uma aula existente.
        /// </summary>
        /// <param name="classDetails">Objeto contendo os dados atualizados da aula.</param>
        /// <returns>A aula atualizada.</returns>
        /// <exception cref="NullReferenceException">Se a aula não for encontrada.</exception>
        public Task<ClassDetails> UpdateClassDetailsAsync(ClassDetails classDetails);

        /// <summary>
        /// Remove uma aula do banco de dados com base no seu ID.
        /// </summary>
        /// <param name="classId">ID da aula a ser removida.</param>
        /// <returns><see langword="true"/> se a remoção for bem-sucedida; caso contrário, <see langword="false"/>.</returns>
        public Task<bool> DeleteClassDetailsAsync(int classId);
    }
}
