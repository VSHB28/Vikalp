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

        [StringLength(10)]
        public string? MobileNumber { get; set; }

        // Plain text password for input only. If your DB stores this column remove [NotMapped].
        [NotMapped]
        public string? Password { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        // DB column is FullName
        [Required]
        [Column("FullName")]
        public string FullName { get; set; } = string.Empty;

        public int? GenderId { get; set; }

        public int? RoleId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        [StringLength(50)]
        public string? LanguageId { get; set; }
    }
}
