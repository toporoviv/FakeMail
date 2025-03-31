using System.Security.Claims;
using FakeMail.Services.Dtos;

namespace FakeMail.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsIdentity> AuthAsync(AuthDto dto, CancellationToken cancellationToken = default);
}