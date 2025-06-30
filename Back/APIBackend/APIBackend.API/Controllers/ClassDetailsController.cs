using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace APIBackend.API.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    [Route("api/classdetails")]
    [ApiController]
    public class ClassDetailsController : ControllerBase
    {
        private readonly IClassDetailsService _classDetailsService;
        private readonly IStudentService _studentService;
        private protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();

        public ClassDetailsController(IClassDetailsService classDetailsService, IStudentService studentService)
        {
            _classDetailsService = classDetailsService;
            _studentService = studentService;
        }

        [HttpPost("createClass")]
        public async Task<IActionResult> CreateClass([FromBody] ClassDetailsDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _classDetailsService.AddClassDetailsAsync(model);

                return Ok(new { result });
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException)
            {
                _loggerNLog.Error($"Erro ao criar aula: {ex.Message}");

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro ao criar aula: {ex.Message}");

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetClassById")]
        public async Task<IActionResult> GetClassById([FromQuery] int classId)
        {
            if (classId <= 0)
            {
                return BadRequest(new { message = $"{classId} Id da aula inválido." });
            }

            try
            {
                var result = await _classDetailsService.GetClassDetailsByIdAsync(classId);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro ao buscar aula: {ex.Message}");

                if (ex is NullReferenceException)
                {
                    return NotFound(new { message = $"Aula com ID {classId} não encontrada." });
                }
                return StatusCode(500, new { message = $"Erro interno ao buscar aula: {ex.Message}" });
            }
        }

        [HttpGet("GetClassesByStudentId")]
        public async Task<IActionResult> GetClassesByStudentId([FromQuery] int studentId)
        {
            if (studentId <= 0 || string.IsNullOrEmpty(studentId.ToString()))
            {
                return BadRequest("Id do estudante inválido.");
            }

            try
            {
                var result = await _classDetailsService.GetClassesDetailsByStudentIdAsync(studentId);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return Ok($" {ex.Message}");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro ao buscar aulas do estudante: {ex.Message}");

                return StatusCode(500, $"Erro interno ao buscar aulas do estudante: {ex.Message}");
            }
        }

        [HttpGet("GetAllClassesByDate")]
        public async Task<IActionResult> GetAllClassesByDate([FromQuery] string? dateFrom, [FromQuery] string? dateTo, [FromQuery] int? studentId = null)
        {
            try
            {
                var result = await _classDetailsService.GetAllClassesDetailsByDateAsync(dateFrom, dateTo, studentId);
                if (result == null || !result.Any())
                {
                    return NotFound("Nenhuma aula encontrada para o período especificado.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro ao buscar aulas por data: {ex.Message}");

                return StatusCode(500, $"Erro interno ao buscar aulas por data: {ex.Message}");
            }
        }

        [HttpPut("UpdateClass")]
        public async Task<IActionResult> UpdateClass([FromBody] ClassDetailsUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Todos os campos obrigatórios devem ser preenchidos.");
            }
            try
            {
                var result = await _classDetailsService.UpdateClassDetailsAsync(model);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("DeleteClass")]
        public async Task<IActionResult> DeleteClass([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id do usuário inválido.");
            }

            try
            {
                var result = await _classDetailsService.DeleteClassDetailsAsync(id);

                return NoContent();
            }
            catch (NullReferenceException ex)
            {
                return NotFound($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor ao deletar aula.{ex.Message}");
            }
        }

        [HttpGet("SummaryClassDetails")]
        public async Task<IActionResult> SummaryClassDetails(string dateFrom, string dateTo, int? studentId = null)
        {
            if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
            {
                return BadRequest("As datas de início e fim são obrigatórias.");
            }

            try
            {
                var result = await _classDetailsService.GetClassesSummaryAsync(dateFrom, dateTo, studentId);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"{ex.Message}");

                return StatusCode(500, $"{ex.Message}");
            }
        }
    }
}
