using System.ComponentModel.DataAnnotations;

namespace FakeMail.MVC.Dtos;

public sealed class AuthDto
{
    [EmailAddress]
    public required string Email { get; init; }
    
    [MinLength(8, ErrorMessage = "Длина пароля должна быть не менее 8 символов")]
    [MaxLength(50, ErrorMessage = "Длина пароля не должна превышать 50 символов")]
    public required string Password { get; init; }
}

public sealed class RegistrationDto
{
    [EmailAddress]
    public required string Email { get; init; }
    
    [MinLength(8, ErrorMessage = "Длина пароля должна быть не менее 8 символов")]
    [MaxLength(50, ErrorMessage = "Длина пароля не должна превышать 50 символов")]
    public required string Password { get; init; }
}