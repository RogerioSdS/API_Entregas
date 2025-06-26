using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace APIBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(iStudentService studentService) : ControllerBase
    {
        private readonly iStudentService _studentService = studentService;
        protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();

        [AllowAnonymous]
        [HttpPost("createStudent")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDTO model)
        {
            // Verifica se o modelo recebido (ex.: DTO com dados enviados pelo cliente) atende às regras de validação definidas
            // (ex.: [Required], [StringLength], [EmailAddress] em propriedades do DTO).
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _studentService.AddStudentAsync(model);
                _loggerNLog.Info($"Usuário criado com sucesso: {result.FirstName} {result.LastName} - {result.Email}");

                return Created("", new { result.FirstName, result.LastName, result.Email });
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException)
            {
                _loggerNLog.Error($"Erro ao criar estudante: {ex.Message}");

                return BadRequest($"Erro ao criar estudante: {ex.Message}");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro inesperado ao criar estudante: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpGet("getStudents")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentService.GetStudentsAsync();

            return Ok(students);
        }


        [HttpGet("getStudentById")]
        public async Task<IActionResult> GetStudentById([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("O ID do estudante deve ser um número positivo.");
            }

            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);

                return Ok(student);
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is NullReferenceException)
            {
                _loggerNLog.Error($"Erro ao buscar estudante por ID: {ex.Message}");

                return NotFound($"Estudante com ID {id} não encontrado.");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro inesperado ao buscar estudante por ID: {ex.Message}");

                return StatusCode(500, "Erro interno do servidor.");
            }

        }

        [HttpGet("getStudentByName")]
        public async Task<IActionResult> GetStudentByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("O nome do estudante deve ser preenchido.");
            }

            try
            {
                var students = await _studentService.GetStudentByNameAsync(name);

                return Ok(students);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                _loggerNLog.Error($"Erro ao buscar estudante por nome: {ex.Message}");

                return NotFound($"Nenhum estudante encontrado com o nome: {name}");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro inesperado ao buscar estudante por nome: {ex.Message}");

                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpPut("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _studentService.UpdateStudentAsync(model);

                return Ok(result);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidOperationException)
            {
                _loggerNLog.Error($"Erro ao buscar estudante por nome: {ex.Message}");

                return NotFound($"Nenhum estudante encontrado com o nome: {model.FirstName}");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro inesperado ao atualizar estudante com o nome: {ex.Message}");

                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpDelete("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("O ID do estudante deve ser um número positivo.");
            }

            try
            {
                var result = await _studentService.DeleteStudentAsync(id);

                return NoContent();
            }
            catch  (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidOperationException)
            {
                _loggerNLog.Error($"Erro inesperado ao deletar estudante com ID {id}: {ex.Message}");

                return NotFound($"Estudante com ID {id} não encontrado.");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro inesperado ao deletar estudante com ID {id}: {ex.Message}");

                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
