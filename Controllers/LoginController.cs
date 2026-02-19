using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Vikalp.Service.Interfaces;
using Vikalp.Helpers;
using Vikalp.Models;

namespace Vikalp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IAuthService _authService;

        public LoginController(IConfiguration configuration, JwtTokenHelper jwtTokenHelper, IAuthService authService)
        {
            _configuration = configuration;
            _jwtTokenHelper = jwtTokenHelper;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.AuthenticateAsync(model.MobileNumber ?? string.Empty, model.Password ?? string.Empty);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Invalid mobile number or password");
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
        new Claim(ClaimTypes.Name, result.FullName),
        new Claim(ClaimTypes.Role, result.RoleName),
        new Claim("Username", result.Username),
        new Claim("RoleId", result.RoleId.ToString()),
        new Claim("Avatar", result.FullName.Substring(0, 1).ToUpper())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //TempData["SuccessMessage"] = "Login successful! Welcome, " + result.FullName;
            return RedirectToAction("Index", "Home");
        }

        // POST: /Login/Logout
        // Signs the user out of the cookie authentication scheme and clears session, then redirects to the login page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out of cookie authentication (this will remove the auth cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear any server-side session state if used
            try
            {
                HttpContext.Session?.Clear();
            }
            catch
            {
                // Ignore if session is not configured
            }

            return RedirectToAction("Login");
        }
    }
}