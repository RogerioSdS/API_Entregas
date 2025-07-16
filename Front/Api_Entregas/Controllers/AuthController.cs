using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // GET api/auth/token-expiry
        [HttpGet("tokensExpiry")]
        public IActionResult GetAccessTokenExpiry()
        {
            // 1) Lê o cookie HttpOnly
            if (!Request.Cookies.TryGetValue("access_token", out var token)
                || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { error = "Access token não encontrado" });
            }

            // 2) Decodifica o JWT (sem validar assinatura)
            JwtSecurityToken jwt;
            try
            {
                jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            }
            catch
            {
                return BadRequest(new { error = "Token inválido" });
            }

            // 3) Retorna a data de expiração
            return Ok(new
            {
                accessTokenExpiresAtUtc = jwt.ValidTo
            });
        }

    }
}