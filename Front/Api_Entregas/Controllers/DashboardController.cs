using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Api_Entregas.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var userDataJson = HttpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userDataJson))
            {
                return RedirectToAction("Login", "Login", new { actionDemanded = "Dashboard" });
            }

            var userData = JsonConvert.DeserializeObject<SignInViewModel>(userDataJson);

            return View("Dashboard", userData);
        }

        /*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }*/
    }
}