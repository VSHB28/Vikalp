using System.ComponentModel.DataAnnotations;

namespace Vikalp.Models.DTO;

public class LoginDto
{
    [Required(ErrorMessage = "Email or Phone Number is required")]
    [StringLength(50, ErrorMessage = "Email or Phone Number cannot exceed 50 characters")]
    public string EmailOrPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
}

public class LoginWithFlagDto
{
    [Required(ErrorMessage = "Email or Phone Number is required")]
    [StringLength(50, ErrorMessage = "Email or Phone Number cannot exceed 50 characters")]
    public string EmailOrPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
    public string Password { get; set; } = string.Empty;

    // 1 => include master cache, 0 => skip
    public int Flag { get; set; } = 0;
}

//public class LoginWithFlagResponseDto
//{
//    public string Token { get; set; } = string.Empty;
//    public UserApiResponseDto User { get; set; } = new();
//    public List<RoleMenuDto> Menus { get; set; } = new();
//    public object? Masterdata { get; set; }
//}

//public class LoginWebResponseDto
//{
//    public UserApiResponseDto User { get; set; } = new();
//    public List<RoleMenuDto> Menus { get; set; } = new();
//    public List<UsersGeographyListDto> UserGeography { get; set; } = new();
//}

//public class UserMasterCacheResponseDto
//{
//    public UserApiResponseDto User { get; set; } = new();
//    public List<UsersGeographyListDto> UserGeography { get; set; } = new();
//    public object Masterdata { get; set; } = new { };
//    public DateTime? CachedOn { get; set; }
//}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class SpjsonUserResponseDto
{
  
    public object UserData { get; set; } = new { };
   
}
