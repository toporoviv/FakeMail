using System.Diagnostics;
using FakeMail.MVC.Dtos;
using Microsoft.AspNetCore.Mvc;
using FakeMail.MVC.Models;
using FakeMail.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeMail.MVC.Controllers;

[Authorize]
public class HomeController(IMailService mailService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var email = User.Identity!.Name!;
        var mailMessages = await mailService.GetMailMessagesByEmailAsync(email);
        return View(mailMessages);
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteMails(List<string> selectedIds)
    {
        var tasks = new List<Task>();

        foreach (var id in selectedIds)
        {
            tasks.Add(Task.Run(async () =>
            {
                await mailService.DeleteMessageAsync(id);
            }));
        }

        await Task.WhenAll(tasks);
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SendMail(SendMailRequest request)
    {
        var senderEmail = User.Identity?.Name;
        if (senderEmail is null)
            return BadRequest();
        
        if (!ModelState.IsValid)
            return View("Index", await mailService.GetMailMessagesByEmailAsync(senderEmail));
        
        foreach (var file in request.Attachments)
        {
            if (file.Length > 5 * 1024 * 1024) // 5 мб
                ModelState.AddModelError("", $"Файл {file.FileName} слишком большой");
        
            var allowedTypes = new[] { "txt", "pdf", "doc", "docx", "jpg", "jpeg", "png" };
            var fileExt = Path.GetExtension(file.FileName).TrimStart('.');
            if (!allowedTypes.Contains(fileExt))
                ModelState.AddModelError("", $"Недопустимый формат файла: {file.FileName}");
        }

        if (!ModelState.IsValid)
            return View("Index", await mailService.GetMailMessagesByEmailAsync(senderEmail));
        
        var messageDto = new MessageDto(
            senderEmail,
            request.Title,
            request.Message,
            request.ReceiverEmail,
            request.Attachments
        );

        await mailService.SendMessageAsync(messageDto);
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> DownloadFile(string mailId, string fileName)
    {
        var mail = await mailService.GetMailMessageByIdAsync(mailId);
        var attachment = mail.Attachments.FirstOrDefault(a => a.FileName == fileName);

        if (attachment == null)
            return NotFound();

        return File(attachment.Data, attachment.ContentType, attachment.FileName);
    }
}