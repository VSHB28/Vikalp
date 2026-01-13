using System.Threading.Tasks;

namespace Vikalp.Service.Interfaces
{
    public class AuthResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public System.Guid UserId { get; init; }
        public int RoleId { get; init; }
        public string Username { get; init; } = string.Empty;
    }

    public interface IAuthService
    {
        Task<AuthResult> AuthenticateAsync(string username, string password);
    }
}