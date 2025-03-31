using System.Text.Json.Serialization;
using FakeMail.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FakeMail.MVC.Controllers.Api;

[ApiController]
[Route("api")]
public class ApiController(IMailService mailService) : ControllerBase
{
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var senderEmail = await mailService.GetMailByTokenAsync(request.Token);

            var tasks = new List<Task>();
            
            foreach (var receiverEmail in request.ReceiverEmails)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await mailService.SendMessageAsync(
                        new MessageDto(senderEmail.Email, request.Title, request.Message, receiverEmail, []));
                }));
            }

            await Task.WhenAll(tasks);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("get-email-by-token")]
    public async Task<IActionResult> GetEmailByToken()
    {
        try
        {
            if (!Request.Headers.TryGetValue("X-Token", out var tokenHeader) 
                || string.IsNullOrWhiteSpace(tokenHeader))
            {
                return BadRequest("Token is missing in the header.");
            }

            var token = tokenHeader.ToString();
            
            var mail = await mailService.GetMailByTokenAsync(token);
            
            return Ok(new GetEmailByTokenResponse
            {
                Email = mail.Email
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

public class SendMessageRequest
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
    
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("receiverEmails")] 
    public string[] ReceiverEmails { get; init; } = [];
}

public class GetEmailByTokenRequest
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}

public class GetEmailByTokenResponse
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}