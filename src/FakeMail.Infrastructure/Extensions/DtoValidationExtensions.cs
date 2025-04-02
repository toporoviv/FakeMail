using System.Net;
using FakeMail.Domain.Exceptions;
using FakeMail.Repositories.Dtos;
using Microsoft.Extensions.Logging;

namespace FakeMail.Repositories.Extensions;

internal static class DtoValidationExtensions
{
    public static void ShouldBeValid<T>(this CreateMailMessageDto dto, ILogger<T> logger)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Message);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.ReceiverEmail);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.SenderEmail);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Title);
        }
        catch (ArgumentException e)
        {
            logger.LogError(e.Message);
            throw new ExceptionWithStatusCode("Некорректный запрос", HttpStatusCode.BadRequest, e);
        }
    }

    public static void ShouldBeValid<T>(this UpdateMailMessageDto dto, ILogger<T> logger)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.ReceiverEmail);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.SenderEmail);
            ArgumentException.ThrowIfNullOrWhiteSpace(dto.Message);
        }
        catch (ArgumentException e)
        {
            logger.LogError(e.Message);
            throw new ExceptionWithStatusCode("Некорректный запрос", HttpStatusCode.BadRequest, e);
        }
    }

    public static void ShouldBeValid<T>(this CreateMailDto dto, ILogger<T> logger)
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

    public static void ShouldBeValid<T>(this UpdateMailDto dto, ILogger<T> logger)
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
}