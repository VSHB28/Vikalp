using System;

namespace Vikalp.Models.DTO
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IsActive { get; set; }
    }
}