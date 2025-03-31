using System.Security.Claims;
using FakeMail.Services.Dtos;

namespace FakeMail.Services.Interfaces;

public interface IRegistrationService
{
    Task<ClaimsIdentity> RegistrationAsync(RegistrationDto dto, CancellationToken cancellationToken = default);
}