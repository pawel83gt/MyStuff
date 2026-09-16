using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Extensions;
using Razor.Models;

namespace Razor.Pages.Tasks
{
    public class DashboardModel : BasePageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }
        public int TotalCount { get; set; }

        public int CompletedCount { get; set; }

        public int ActiveCount { get; set; }

        public Dictionary<TodoCategory, int> CategoryCounts { get; set; } = new();

        // 👇 СПИСОК ДЛЯ ЧАСТИЧНОГО ПРЕДСТАВЛЕНИЯ
        public List<StatLink> StatLinks { get; set; } = new();
        public List<StatLink> CategoryLinks { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalCount = await _context.TodoItems.Where(x => x.UserId == CurrentUserId).CountAsync();

            CompletedCount = await _context.TodoItems
                .CountAsync(x => x.UserId == CurrentUserId && x.IsCompleted);

            ActiveCount = await _context.TodoItems
                .CountAsync(x => x.UserId == CurrentUserId && !x.IsCompleted);

            CategoryCounts = await _context.TodoItems
                .Where(x => x.UserId == CurrentUserId)
                .GroupBy(x => x.TodoCategory)
                .ToDictionaryAsync(
                    x => x.Key,
                    x => x.Count());

            // ✅ ЗДЕСЬ ФОРМИРУЕМ СПИСОК ДЛЯ ЧАСТИЧНОГО ПРЕДСТАВЛЕНИЯ
            StatLinks = new List<StatLink>
{
    new()
    {
        Label = "Всего",
        Count = TotalCount,
        Page = "/Tasks/Index"
    },

    new()
    {
        Label = "Незавершенные",
        Count = ActiveCount,
        Page = "/Tasks/Index",
        RouteStatus = "active"
    },

    new()
    {
        Label = "Завершенные",
        Count = CompletedCount,
        Page = "/Tasks/Index",
        RouteStatus = "completed"
    }
};

            CategoryLinks = CategoryCounts
                .Where(c => c.Value > 0)
                .Select(c => new StatLink
                {
                    Label = c.Key.GetDisplayName(),
                    Count = c.Value,
                    Page = "/Tasks/Index",
                    RouteCategory = c.Key.ToString(),
                    Type = StatLinkType.Category,
                    Icon = c.Key.ToString()
                })
        .ToList();
        }
    }
}
