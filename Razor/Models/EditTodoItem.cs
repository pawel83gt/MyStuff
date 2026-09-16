using System.ComponentModel.DataAnnotations;

namespace Razor.Models
{
    public class EditTodoItem
    {
        [Required(ErrorMessage = "Описание обязательно")]
        [MinLength(15, ErrorMessage = "Описание должно содержать минимум 15 символов")]
        [MaxLength(500, ErrorMessage = "Описание не длиннее 500 символов")]
        public string Description { get; set; } = string.Empty;
    }
}
