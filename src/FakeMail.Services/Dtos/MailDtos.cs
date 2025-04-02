using Microsoft.AspNetCore.Http;

namespace FakeMail.Services.Dtos;

public record struct MessageDto(
    string Sender,
    string Title,
    string Message,
    string Receiver,
    ICollection<IFormFile> Attachments);