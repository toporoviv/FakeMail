using System.ComponentModel.DataAnnotations;

namespace FakeMail.MVC.Dtos;

public class SendMailRequest
{
    [Required(ErrorMessage = "Email получателя обязателен")]
    public required string ReceiverEmail { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Текст сообщения обязателен")]
    public required string Message { get; set; }

    public ICollection<IFormFile> Attachments { get; set; } = new List<IFormFile>();
}