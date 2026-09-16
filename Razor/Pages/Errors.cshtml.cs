using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor.Pages
{
    public class _404Model : PageModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public void OnGet(int code)
        {
            StatusCode = code;

            Message = code switch
            {
                400 => "Некорректный запрос. Проверьте введённые данные.",
                401 => "Необходима авторизация для доступа к этой странице.",
                403 => "У вас нет прав для просмотра этой страницы.",
                404 => "Страница не найдена. Проверьте адрес.",
                500 => "Внутренняя ошибка сервера. Попробуйте позже.",
                _ => $"Произошла ошибка (код {code}). Попробуйте позже."
            };

            // Получаем оригинальный путь, на котором произошла ошибка
            if (Request.Query.ContainsKey("originalPath"))
            {
                Path = Request.Query["originalPath"];
            }
        }
    }
}
