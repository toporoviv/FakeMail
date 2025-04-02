using System.Net;
using FakeMail.Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FakeMail.MVC.Extensions;

internal static class ExceptionHandlerExtensions
{
    public static IActionResult Handle(this ExceptionWithStatusCode e) => e.StatusCode switch
    {
        HttpStatusCode.NotFound => new NotFoundObjectResult(e.Message),
        HttpStatusCode.BadRequest => new BadRequestObjectResult(e.Message),
        _ => new BadRequestObjectResult("Что то пошло не так")
    };
}