namespace FakeMail.Services.Dtos;

public record struct AuthDto(string Email, string Password);
public record struct RegistrationDto(string Email, string Password);