using System;

namespace Vikalp.Models.DTO
{
    public class UserDto
    {
        // Identity primary key
        public int UserId { get; set; }

        // Login credentials
        public string? MobileNumber { get; set; }   // Used for login
        public string? Password { get; set; }       // Plain password (only for input, never store)
        public string? PasswordHash { get; set; }   // Stored in DB

        // Profile fields
        public string FullName { get; set; }
        public string? Email { get; set; }
        public int? GenderId { get; set; }
        public int? RoleId { get; set; }
        public List<int> LanguageId { get; set; }

        // Status
        public int IsActive { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public string? RoleName { get; set; }


        // Location
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int? BlockId { get; set; }
    }
}
