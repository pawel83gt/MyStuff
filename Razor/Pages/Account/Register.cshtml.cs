using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Models;

namespace Razor.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public RegisterModel(UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Введите корректный адрес электронной почты.")]
        [Required(ErrorMessage = "Введите адрес вашей электронной почты")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Допускается латинские большие и маленькие буквы, цифры, не менее 8 символов")]
        [Required(ErrorMessage = "Придумайте пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Повторите пароль.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = string.Empty;
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
        var checkUser = await _userManager.FindByEmailAsync(Input.Email);
        if (checkUser != null)
        {
            ModelState.AddModelError("Input.Email", "Пользователь с таким email существует.");
            _logger.LogWarning(
            "Неудачная попытка регистрации. Email: {Email}. Причина: Пользователь существует",
            Input.Email);
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email
        };

        var result = await _userManager.CreateAsync(
            user,
            Input.Password);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        foreach (var error in result.Errors)
        {
            _logger.LogWarning(
        "Ошибка регистрации. Email: {Email}. Code: {Code}. Description: {Description}",
        Input.Email,
        error.Code,
        error.Description);

            ModelState.AddModelError(
                string.Empty,
                error.Description);
        }

        return Page();
    }
}