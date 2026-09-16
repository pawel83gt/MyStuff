using System.ComponentModel.DataAnnotations;

namespace Razor.Models
{
    public class CreateTodoItem
    {
        [Required(ErrorMessage = "Заголовок обязателен")]
        [MaxLength(100, ErrorMessage = "Заголово должен быть не длиннее 100 символов")]
        [MinLength(3, ErrorMessage = "Заголовок должен содержать минимум 3 символа")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно")]
        [MinLength(15, ErrorMessage = "Заголовок должен содержать минимум 15 символов")]
        public string Description { get; set; } = string.Empty;

        public TodoCategory TodoCategory { get; set; }
    }
}
