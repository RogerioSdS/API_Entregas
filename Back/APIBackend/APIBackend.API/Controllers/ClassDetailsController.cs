using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace APIBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassDetailsController : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin")]
        [Route("api/[controller]")]
        [ApiController]
        public class AdminController : ControllerBase
        {
            private readonly IClassDetailsService _classDetailsService;
            private readonly IStudentService _studentService;
            private protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();

            public AdminController(IClassDetailsService classDetailsService, IStudentService studentService)
            {
                _classDetailsService = classDetailsService;
                _studentService = studentService;
            }

            [HttpGet("createClass")]
            public async Task<IActionResult> CreateClass([FromBody] ClassDetailsDTO model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    var result = await _classDetailsService.AddClassDetailsAsync(model);

                    return Ok(result);
                }
                catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException)
                {
                    _loggerNLog.Error($"Erro ao criar aula: {ex.Message}");
                    return BadRequest($"Erro ao criar aula: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _loggerNLog.Error($"Erro ao criar aula: {ex.Message}");
                    return StatusCode(500, "Erro interno ao criar aula.");
                }
            }

            [HttpGet("GetClassById")]
            public async Task<IActionResult> GetClassById([FromQuery] int classId)
            {
                if (classId <= 0)
                {
                    return BadRequest("Id da aula inválido.");
                }

                try
                {
                    var result = await _classDetailsService.GetClassDetailsByIdAsync(classId);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _loggerNLog.Error($"Erro ao buscar aula: {ex.Message}");
                    return StatusCode(500, $"Erro interno ao buscar aula: {ex.Message}");
                }
            }

            [HttpPut("UpdateClass")]
            public async Task<IActionResult> UpdateClass([FromBody] ClassUpdateFromAdminDTO model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Todos os campos obrigatórios devem ser preenchidos.");
                }
                try
                {
                    var result = await _ClassService.UpdateClassFromAdminAsync(model);

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
                    var result = await _ClassService.DeleteClassAsync(id);

                    return NoContent();
                }
                catch (NullReferenceException)
                {
                    return NotFound("Usuário não encontrado.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro interno do servidor ao deletar usuário.{ex.Message}");
                }
            }

            [HttpDelete("DeleteStudent")]
            public async Task<IActionResult> DeleteStudent([FromQuery] int id)
            {
                if (id <= 0)
                {
                    return BadRequest("Id do usuário inválido.");
                }

                try
                {
                    var result = await _studentService.DeleteStudentAsync(id);

                    return NoContent();
                }
                catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidOperationException)
                {
                    return BadRequest($"Erro ao deletar estudante: {ex.Message}");
                }
                catch (NullReferenceException)
                {
                    return NotFound("Estudante não encontrado.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
                }
            }


        }
    }
}