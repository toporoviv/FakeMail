using System.Security.Claims;
using FakeMail.MVC.Dtos;
using FakeMail.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FakeMail.MVC.Controllers;

[Route("auth")]
public class AuthController(IAuthService authService, IRegistrationService registrationService) : Controller
{
    [HttpGet("login")]
    public IActionResult Login() => View();

    [HttpGet("registration")]
    public IActionResult Registration() => View();
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthDto dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var response = await authService.AuthAsync(new Services.Dtos.AuthDto(dto.Email, dto.Password));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(response));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
        }

        return View(dto);
    }
    
    [HttpPost("registration")]
    public async Task<IActionResult> Registration(RegistrationDto dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var response = await registrationService
                    .RegistrationAsync(new Services.Dtos.RegistrationDto(dto.Email, dto.Password));
            
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(response));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
        }

        return View(dto);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}