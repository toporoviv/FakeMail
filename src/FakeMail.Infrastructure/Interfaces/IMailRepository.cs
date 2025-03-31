using FakeMail.Domain.Entities.Mails;
using FakeMail.Repositories.Dtos;

namespace FakeMail.Repositories.Interfaces;

public interface IMailRepository
{
    Task<Mail> CreateAsync(CreateMailDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mail>> GetAsync(CancellationToken cancellationToken = default);
    Task<Mail?> GetAsync(string email, CancellationToken cancellationToken = default);
    Task UpdateAsync(string email, UpdateMailDto dto, CancellationToken cancellationToken = default);
}