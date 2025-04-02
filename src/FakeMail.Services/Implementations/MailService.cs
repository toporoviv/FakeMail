using System.Net;
using FakeMail.Domain.Entities.Mails;
using FakeMail.Domain.Exceptions;
using FakeMail.Repositories.Dtos;
using FakeMail.Repositories.Interfaces;
using FakeMail.Services.Dtos;
using FakeMail.Services.Extensions;
using FakeMail.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FakeMail.Services.Implementations;

internal class MailService(
    IMailRepository mailRepository,
    IMailMessageRepository mailMessageRepository,
    ILogger<IMailService> logger) : IMailService
{
    public Task<IEnumerable<MailMessage>> GetMailMessagesByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return mailMessageRepository
            .GetAsync(new GetMailMessageDto(null, ReceiverEmail: email), cancellationToken);
    }

    public async Task<Mail> GetMailByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var mail = await mailRepository.GetAsync(email, cancellationToken);

        if (mail is null)
        {
            throw new ExceptionWithStatusCode($"Пользователь с email={email} не найден", HttpStatusCode.NotFound);
        }
        
        return mail;
    }

    public async Task<Mail> GetMailByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogError("Token не должен быть пустым или равным null");
            throw new ExceptionWithStatusCode("Что то пошло не так", HttpStatusCode.BadRequest);
        }
        
        var mail = (await mailRepository.GetAsync(cancellationToken))
            .FirstOrDefault(mail => mail.Token == token);

        if (mail is null)
        {
            throw new ExceptionWithStatusCode($"Token={token} не валиден", HttpStatusCode.BadRequest);
        }

        return mail;
    }

    public async Task SendMessageAsync(MessageDto dto, CancellationToken cancellationToken = default)
    {
        dto.ShouldBeValid(logger);

        var receiver = await mailRepository.GetAsync(dto.Receiver, cancellationToken);

        if (receiver is null)
        {
            throw new ExceptionWithStatusCode($"Пользователь с email={dto.Receiver} не найден", HttpStatusCode.NotFound);
        }
        
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

    public Task DeleteMessageAsync(string id, CancellationToken cancellationToken)
    { 
        return mailMessageRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<MailMessage> GetMailMessageByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogError("Id не должен быть пустым или равным null");
            throw new ExceptionWithStatusCode("Что то пошло не так", HttpStatusCode.BadRequest);
        }

        var mailMessage = await mailMessageRepository.GetAsync(id, cancellationToken);

        if (mailMessage is null)
        {
            logger.LogError($"Сообщение с id={id} не найдено");
            throw new ExceptionWithStatusCode("Что то пошло не так", HttpStatusCode.BadRequest);
        }

        return mailMessage;
    }
}