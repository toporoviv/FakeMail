using System.Net;
using FakeMail.Domain.Exceptions;
using FakeMail.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace FakeMail.Services.Extensions;

internal static class DtoValidationExtensions
{
    public static void ShouldBeValid<T>(this AuthDto dto, ILogger<T> logger)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Email);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);
        }
        catch (ArgumentException e)
        {
            logger.LogError(e.Message);
            throw new ExceptionWithStatusCode("Некорректный запрос", HttpStatusCode.BadRequest, e);
        }
    }
    
    public static void ShouldBeValid<T>(this RegistrationDto dto, ILogger<T> logger)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Email);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);
        }
        catch (ArgumentException e)
        {
            logger.LogError(e.Message);
            throw new ExceptionWithStatusCode("Некорректный запрос", HttpStatusCode.BadRequest, e);
        }
    }
    
    public static void ShouldBeValid<T>(this MessageDto dto, ILogger<T> logger)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Message);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Sender);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Title);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Receiver);
        }
        catch (ArgumentException e)
        {
            logger.LogError(e.Message);
            throw new ExceptionWithStatusCode("Некорректный запрос", HttpStatusCode.BadRequest, e);
        }
    }
}