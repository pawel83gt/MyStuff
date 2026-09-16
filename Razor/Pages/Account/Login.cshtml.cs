using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;

namespace Razor.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты.")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // 1. Проверяем, существует ли пользователь с таким email
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ModelState.AddModelError("Input.Email", "Пользователь с таким email не найден.");
            _logger.LogWarning(
            "Неудачная попытка входа. Email: {Email}. Причина: Пользователь не найден",
            Input.Email);
            return Page();
        }

        // 2.Проверяем пароль
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, Input.Password);
        if (!isPasswordValid)
        {
            ModelState.AddModelError("Input.Password", "Неверный пароль.");
            _logger.LogWarning(
            "Неудачная попытка входа пользователя с Email: {Email}. Причина: Пароль не верный",
            Input.Email);
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation(
                "Успешный вход. Пользователь: {Email}",
                Input.Email
            );
            return RedirectToPage("/Index");
        }

        _logger.LogWarning(
            "Неудачная попытка входа. Email: {Email}. Причина: {Reason}",
            Input.Email,
            GetFailureReason(result)
        );

        ModelState.AddModelError(
            string.Empty,
            "Неверный email или пароль.");

        return Page();
    }

    private string GetFailureReason(Microsoft.AspNetCore.Identity.SignInResult result)
    {
        if (result.IsLockedOut) return "Учётная запись заблокирована";
        if (result.IsNotAllowed) return "Учётная запись не подтверждена";
        if (result.RequiresTwoFactor) return "Требуется двухфакторная аутентификация";
        return "Неверный email или пароль";
    }
}