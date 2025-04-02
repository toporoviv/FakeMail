using System.Net.Http.Json;
using FakeMail.Domain.Entities.Mails;
using FakeMail.MVC.Controllers.Api;
using FakeMail.Services.Dtos;
using FakeMail.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FakeMail.Tests;

public class ApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMailService> _mockMailService;

    public ApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mockMailService = new Mock<IMailService>();
    }

    [Fact]
    public async Task SendMessage_ValidRequest_ReturnsOk()
    {
        // Arrange
        var senderEmail = "sender@example.com";
        var receiverEmails = new[] { "receiver1@example.com", "receiver2@example.com" };
        var request = new SendMessageRequest
        {
            Token = "valid-token",
            Title = "Test Title",
            Message = "Test Message",
            ReceiverEmails = receiverEmails
        };

        _mockMailService
            .Setup(service => service
                .GetMailByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mail { Email = senderEmail, Password = "password", Token = "valid-token" });

        _mockMailService
            .Setup(service => service
                .SendMessageAsync(It.IsAny<MessageDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var client = CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/send-message", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        _mockMailService.Verify(service => service.GetMailByTokenAsync(request.Token, CancellationToken.None), Times.Once);
        foreach (var receiverEmail in receiverEmails)
        {
            _mockMailService.Verify(service => service.SendMessageAsync(
                It.Is<MessageDto>(dto =>
                    dto.Sender == senderEmail &&
                    dto.Title == request.Title &&
                    dto.Message == request.Message &&
                    dto.Receiver == receiverEmail),
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }

    [Fact]
    public async Task SendMessage_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            Token = "invalid-token",
            Title = "Test Title",
            Message = "Test Message",
            ReceiverEmails = new[] { "receiver@example.com" }
        };

        _mockMailService
            .Setup(service => service
                .GetMailByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Invalid token"));

        var client = CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/send-message", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid token", responseBody);

        _mockMailService
            .Verify(service => service.GetMailByTokenAsync(request.Token, CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task GetEmailByToken_ValidToken_ReturnsEmail()
    {
        // Arrange
        var token = "valid-token";
        var expectedEmail = "test@example.com";

        _mockMailService.Setup(service => service.GetMailByTokenAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mail { Email = expectedEmail, Password = "pass", Token = token });

        var client = CreateClient();

        // Act
        var response = await client.GetAsync($"/api/get-email-by-token?token={token}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GetEmailByTokenResponse>();

        Assert.NotNull(result);
        Assert.Equal(expectedEmail, result.Email);

        _mockMailService.Verify(service => service.GetMailByTokenAsync(token, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetEmailByToken_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var token = "invalid-token";
        var errorMessage = "Invalid token.";

        _mockMailService.Setup(service => service.GetMailByTokenAsync(token, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        var client = CreateClient();

        // Act
        var response = await client.GetAsync($"/api/get-email-by-token?token={token}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains(errorMessage, responseBody);

        _mockMailService.Verify(service => service.GetMailByTokenAsync(token, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetEmailByToken_MissingToken_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/api/get-email-by-token");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", responseBody, StringComparison.OrdinalIgnoreCase);
    }
    
    private HttpClient CreateClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mockMailService.Object);
            });
        }).CreateClient();
    }
}