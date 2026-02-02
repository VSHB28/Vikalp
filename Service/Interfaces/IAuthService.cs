using System.Threading.Tasks;

namespace Vikalp.Service.Interfaces
{
    public class AuthResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }

        // changed to int to match DB identity
        public int UserId { get; init; }

        public int RoleId { get; init; }
        public string Username { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string RoleName { get; init; } = string.Empty;

    }

    public interface IAuthService
    {
        Task<AuthResult> AuthenticateAsync(string username, string password);
    }
}