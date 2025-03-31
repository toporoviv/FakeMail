using System.Collections;
using FakeMail.Domain.Entities.Mails;
using Microsoft.AspNetCore.Http;

namespace FakeMail.Repositories.Dtos;

public record struct CreateMailMessageDto(
    string SenderEmail,
    string Title,
    string Message,
    string ReceiverEmail,
    ICollection<FileAttachment> Attachments);
public record struct GetMailMessageDto(string? SenderEmail, string? ReceiverEmail);
public record struct UpdateMailMessageDto(string SenderEmail, string Message, string ReceiverEmail, DateTime CreatedAt);