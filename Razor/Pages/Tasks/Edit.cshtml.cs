using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Pages.Tasks;

public class EditModel : BasePageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public EditModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public TodoItem TodoItem { get; set; } = null!;

    [BindProperty]
    public EditTodoItem EditTodoItem { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == CurrentUserId);

        if (todoItem == null)
        {
            return NotFound();
        }

        TodoItem = todoItem;
        EditTodoItem.Description = TodoItem.Description ?? string.Empty;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        // 1. Загружаем сущность (один раз!)
        var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x =>
            x.Id == id && x.UserId == CurrentUserId);

        if (todoItem == null)
        {
            return NotFound();
        }

        // 2. Проверяем валидацию DTO
        if (!ModelState.IsValid)
        {
            TodoItem = todoItem; // ← используем уже загруженную сущность
            return Page();
        }

        // 3. Обновляем описание
        todoItem.Description = EditTodoItem.Description;

        // 4. Сохраняем
        await _context.SaveChangesAsync();

        _logger.LogInformation("Пользователь: {UserId} изменил запись {Title}", CurrentUserId, todoItem.Title);

        return RedirectToPage("./Details", new { id });
    }
}