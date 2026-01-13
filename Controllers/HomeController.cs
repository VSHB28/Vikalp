using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Vikalp.Controllers
{
    /// <summary>
    /// Controller for the application home page.
    /// Requires an authenticated user by default.
    /// </summary>
    [Authorize]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) =>
            _logger = logger;

        /// <summary>
        /// Render the home page.
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Home Index requested by user {User}", User?.Identity?.Name ?? "anonymous");

            // If you need to load a view model, do it here with async IO and cancellationToken.
            await Task.CompletedTask; // placeholder for real async work

            return View();
        }

        /// <summary>
        /// Error page (allow anonymous so error details can be shown for unauthenticated requests).
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            // Return a view named "Error" and pass the request id as the model.
            return View("Error", requestId);
        }

        public IActionResult dashboard()
        {
            return View();
        }
    }
}