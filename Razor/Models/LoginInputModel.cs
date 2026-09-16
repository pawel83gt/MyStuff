using System.ComponentModel.DataAnnotations;

namespace Razor.Models
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты.")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
