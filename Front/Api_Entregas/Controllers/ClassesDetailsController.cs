using Microsoft.AspNetCore.Mvc;

namespace Api_Entregas.Controllers
{
    public class ClassesDetailsController : Controller
    {
        private readonly ILogger<AuthController> _logger;

        public ClassesDetailsController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }
        
        public ActionResult Index()
        {
            return View();
        }

    }
}
