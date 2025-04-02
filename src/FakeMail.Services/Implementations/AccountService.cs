using System.Net;
using System.Security.Claims;
using FakeMail.Domain.Entities.Mails;
using FakeMail.Domain.Exceptions;
using FakeMail.Repositories.Dtos;
using FakeMail.Repositories.Interfaces;
using FakeMail.Services.Dtos;
using FakeMail.Services.Extensions;
using FakeMail.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FakeMail.Services.Implementations;

internal class AccountService(
    IMailRepository mailRepository,
    ILogger<IAuthService> authLogger,
    ILogger<IRegistrationService> registrationLogger) : IAuthService, IRegistrationService
{
    public async Task<ClaimsIdentity> AuthAsync(AuthDto dto, CancellationToken cancellationToken = default)
    {
        dto.ShouldBeValid(authLogger);

        var mail = await mailRepository.GetAsync(dto.Email, cancellationToken);
                
        if (mail is null)
        {
            authLogger.LogError($"Пользователь с указанным email={dto.Email} не найден");
            throw new ExceptionWithStatusCode("Пользователь с указанным email не найден", HttpStatusCode.NotFound);
        }

        if (mail.Password != dto.Password)
        {
            authLogger.LogError($"{mail.Password}!={dto.Password}");
            throw new ExceptionWithStatusCode("Неправильный логин или пароль", HttpStatusCode.BadRequest);
        }

        return Authenticate(mail);
    }

    public async Task<ClaimsIdentity> RegistrationAsync(
        RegistrationDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.ShouldBeValid(registrationLogger);

        var mail = await mailRepository.GetAsync(dto.Email, cancellationToken);

        if (mail is not null)
        {
            registrationLogger.LogError($"Почта {dto.Email} уже существует");
            throw new ExceptionWithStatusCode("Данная почта уже существует", HttpStatusCode.BadRequest);
        }

        var createdMail = await mailRepository
            .CreateAsync(new CreateMailDto(dto.Email, dto.Password), cancellationToken);

        return Authenticate(createdMail);
    }
    
    private ClaimsIdentity Authenticate(Mail mail)
    {
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, mail.Email)
        };

        return new ClaimsIdentity(claims, "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
    }
}