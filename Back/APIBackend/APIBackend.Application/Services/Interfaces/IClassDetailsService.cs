using System;
using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces
{
    public interface IClassDetailsService
    {
        /// <summary>
        /// Cria uma nova aula no sistema.
        /// </summary>
        /// <param name="classDetails">Objeto DTO contendo os dados da aula a ser criada.</param>
        /// <returns>Retorna o <see cref="ClassDetailsDTO"/> da aula criada.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro inesperado ao adicionar a aula.</exception>
        public Task<ClassDetailsDTO> AddClassDetailsAsync(ClassDetailsDTO classDetails);

        /// <summary>
        /// Busca uma aula pelo seu ID.
        /// </summary>
        /// <param name="classId">ID da aula.</param>
        /// <returns>
        /// Retorna um objeto <see cref="ClassDetailsDTO"/> com os dados da aula,
        /// ou <see langword="null"/> se não encontrada.
        /// </returns>
        public Task<ClassDetailsDTO?> GetClassDetailsByIdAsync(int classId);

        /// <summary>
        /// Busca uma aula pelo ID do aluno.
        /// </summary>
        /// <param name="studentId">ID do aluno.</param>
        /// <returns>
        /// Retorna um objeto <see cref="ClassDetailsDTO"/> com os dados da aula associada ao aluno,
        /// ou <see langword="null"/> se não encontrada.
        /// </returns>
        public Task<ClassDetailsDTO?> GetClassDetailsByStudentIdAsync(int studentId);

        /// <summary>
        /// Busca todas as aulas de um professor em um intervalo de datas.
        /// </summary>
        /// <param name="dateFrom">Data inicial do intervalo.</param>    
        /// <param name="dateTo">Data final do intervalo.</param>
        /// <param name="studentId">ID do aluno (opcional).</param>
        /// <returns>
        /// Retorna uma lista de objetos <see cref="ClassDetailsDTO"/> com os dados das aulas encontradas,
        /// ou uma lista vazia se nenhuma aula for encontrada.
        /// </returns>
        public Task<IEnumerable<ClassDetailsDTO>?> GetAllClassesDetailsByDateAsync(string? dateFrom, string? dateTo, int? studentId = null);

        /// <summary>
        /// Atualiza uma aula existente no sistema.
        /// </summary>
        /// <param name="classDetails">Objeto DTO contendo os dados atualizados da aula.</param>
        /// <returns>Retorna o <see cref="ClassDetailsDTO"/> com os dados atualizados.</returns>
        /// <exception cref="NullReferenceException">Lançada se a aula não for encontrada.</exception>
        public Task<ClassDetailsDTO> UpdateClassDetailsAsync(ClassDetailsUpdateDTO classDetails);

        /// <summary>
        /// Exclui uma aula com base no ID fornecido.
        /// </summary>
        /// <param name="classId">ID da aula a ser excluída.</param>
        /// <returns>Retorna <see langword="true"/> se a exclusão for bem-sucedida.</returns>
        /// <exception cref="NullReferenceException">Lançada se a aula não for encontrada.</exception>
        public Task<bool> DeleteClassDetailsAsync(int classId);
    }
}
