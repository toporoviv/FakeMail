using FakeMail.Domain.Entities.Mails;
using FakeMail.Services.Dtos;
using Microsoft.AspNetCore.Http;

namespace FakeMail.Services.Interfaces;

public interface IMailService
{
    Task<IEnumerable<MailMessage>> GetMailMessagesByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Mail> GetMailByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Mail> GetMailByTokenAsync(string token, CancellationToken cancellationToken = default); 
    Task SendMessageAsync(MessageDto dto, CancellationToken cancellationToken = default);
    Task DeleteMessageAsync(string id, CancellationToken cancellationToken = default);

    Task<MailMessage> GetMailMessageByIdAsync(string id, CancellationToken cancellationToken = default);
}