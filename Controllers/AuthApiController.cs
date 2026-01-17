using Microsoft.AspNetCore.Mvc;
using Vikalp.Service.Interfaces;
using Vikalp.Helpers;
using Vikalp.Models;

namespace Vikalp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthApiController(IAuthService authService, JwtTokenHelper jwtTokenHelper)
        {
            _authService = authService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { message = "Mobile number and password required" });

            // Authenticate using MobileNumber
            var result = await _authService.AuthenticateAsync(model.MobileNumber ?? string.Empty, model.Password ?? string.Empty);
            if (!result.Success)
                return Unauthorized(new { message = result.Message ?? "Invalid mobile number or password" });

            var token = _jwtTokenHelper.GenerateToken(result.UserId, result.Username, result.RoleId);

            return Ok(new
            {
                token = token,
                userId = result.UserId,
                username = result.Username,
                roleId = result.RoleId
            });
        }
    }
}
