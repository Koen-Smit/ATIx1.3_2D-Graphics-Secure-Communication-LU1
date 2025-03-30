﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class AccountRequest
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Wachtwoord moet minimaal 10 tekens lang zijn.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{10,}$",
        ErrorMessage = "Wachtwoord moet minstens één kleine letter, hoofdletter, cijfer en speciaal teken bevatten.")]
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
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class LoginResponse
{
    public string? Message { get; set; }
    public string? Token { get; set; }
}