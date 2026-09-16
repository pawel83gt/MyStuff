using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;
using System.Security.Claims;

namespace Razor.Pages.Tasks
{
    public class DetailsModel : BasePageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public TodoItem? TodoItem { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Page();
            }

            TodoItem = await _context.TodoItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);

            if (TodoItem == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostToggleCompleteAsync(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Page();
            }

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);

            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.IsCompleted = !todoItem.IsCompleted;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id });
        }
    }
}