namespace FakeMail.Repositories.Dtos;

public record struct CreateMailDto(string Email, string Password);
public record struct UpdateMailDto(string Email, string Password);