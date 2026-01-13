using Microsoft.AspNetCore.Mvc;

namespace Vikalp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult entryform()
        {
            return View();
        }
        public IActionResult beneficarylist()
        {
            return View();
        }
    }
}
