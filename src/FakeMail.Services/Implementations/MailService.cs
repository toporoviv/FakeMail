using FakeMail.Domain.Entities.Mails;
using FakeMail.Repositories.Dtos;
using FakeMail.Repositories.Interfaces;
using FakeMail.Services.Interfaces;

namespace FakeMail.Services.Implementations;

internal class MailService(
    IMailRepository mailRepository,
    IMailMessageRepository mailMessageRepository) : IMailService
{
    public async Task<IEnumerable<MailMessage>> GetMailMessagesByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await mailMessageRepository
            .GetAsync(new GetMailMessageDto(null, ReceiverEmail: email), cancellationToken);
    }

    public async Task<Mail> GetMailByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var mail = (await mailRepository.GetAsync(cancellationToken))
            .FirstOrDefault(mail => mail.Token == token);
        
        if (mail is null)
            throw new Exception($"Token={token} не валиден");

        return mail;
    }

    public async Task SendMessageAsync(MessageDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Message);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Sender);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Title);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Receiver);
        
        var attachments = new List<FileAttachment>();

        foreach (var file in dto.Attachments)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
        
            attachments.Add(new FileAttachment
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = memoryStream.ToArray()
            });
        }

        var createMailMessageDto =
            new CreateMailMessageDto(dto.Sender, dto.Title, dto.Message, dto.Receiver, attachments);
        
        await mailMessageRepository.CreateAsync(createMailMessageDto, cancellationToken);
    }

    public async Task DeleteMessageAsync(string id, CancellationToken cancellationToken)
    {
        await mailMessageRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<MailMessage> GetMailMessageByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var mailMessage = await mailMessageRepository.GetAsync(id, cancellationToken);

        if (mailMessage is null)
            throw new Exception($"Сообщение с id={id} не найдено");

        return mailMessage;
    }
}