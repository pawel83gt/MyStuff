using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Data;
using Razor.Models;
using System.Security.Claims;

namespace Razor.Pages.Tasks
{
    public class CreateModel : BasePageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CreateTodoItem TodoItem { get; set; } = new();

        public TodoCategory[] Categories { get; } = Enum.GetValues<TodoCategory>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var todoItem = new TodoItem
            {
                Id = Guid.CreateVersion7(),

                UserId = CurrentUserId,
                Title = TodoItem.Title,
                Description = TodoItem.Description,
                TodoCategory = TodoItem.TodoCategory,

                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.TodoItems.Add(todoItem);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}