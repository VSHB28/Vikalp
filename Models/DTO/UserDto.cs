using System;

namespace Vikalp.Models.DTO
{
    public class UserDto
    {
        // UserId is a GUID in the database
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // NOTE: Password should be hashed before storing
        public string? Password { get; set; }
        // RoleId left as Guid? earlier if your roles are GUIDs — keep as needed
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
