using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vikalp.Models
{
    [Table("ApiUsers")]
    public class ApiUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public Guid UserGuid { get; set; }

        public string? Password { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public bool Deleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? RoleId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? MobileNumber { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [StringLength(50)]
        public string? LanguageId { get; set; }

        public int? GenderId { get; set; }
    }
}