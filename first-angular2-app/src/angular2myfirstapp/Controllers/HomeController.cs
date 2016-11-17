using Microsoft.AspNetCore.Mvc;

namespace angular2myfirstapp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
