using System.Security.Claims;
using FakeMail.Domain.Entities.Mails;
using FakeMail.Repositories.Dtos;
using FakeMail.Repositories.Interfaces;
using FakeMail.Services.Dtos;
using FakeMail.Services.Interfaces;

namespace FakeMail.Services.Implementations;

internal class AccountService(IMailRepository mailRepository) : IAuthService, IRegistrationService
{
    public async Task<ClaimsIdentity> AuthAsync(AuthDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);

        var mail = await mailRepository.GetAsync(dto.Email, cancellationToken);
                
        if (mail is null)
        {
            throw new Exception("Неправильный логин или пароль");
        }

        if (mail.Password != dto.Password)
        {
            throw new Exception("Неправильный логин или пароль");
        }

        return Authenticate(mail);
    }

    public async Task<ClaimsIdentity> RegistrationAsync(
        RegistrationDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);

        var mail = await mailRepository.GetAsync(dto.Email, cancellationToken);

        if (mail is not null)
        {
            throw new Exception("Данная почта уже существует");
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