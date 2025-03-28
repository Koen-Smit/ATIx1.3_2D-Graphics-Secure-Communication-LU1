using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class AccountRequest
{
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string? Email { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Password must be at least 10 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{10,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, uppercase letter, digit, and special character.")]
    public string? Password { get; set; }
}

public class AppUser : IdentityUser
{
    public override string? Email { get; set; }
    public override string? NormalizedEmail { get; set; }
}

public class LoginRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class LoginResponse
{
    public string? Message { get; set; }
    public string? Token { get; set; }
}