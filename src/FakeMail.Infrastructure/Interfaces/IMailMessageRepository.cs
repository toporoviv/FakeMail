using FakeMail.Domain.Entities.Mails;
using FakeMail.Repositories.Dtos;

namespace FakeMail.Repositories.Interfaces;

public interface IMailMessageRepository
{
    Task<MailMessage> CreateAsync(CreateMailMessageDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<MailMessage>> GetAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MailMessage>> GetAsync(GetMailMessageDto dto, CancellationToken cancellationToken = default);
    Task<MailMessage?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateMailMessageDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}