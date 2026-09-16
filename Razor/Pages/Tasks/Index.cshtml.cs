using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Pages.Tasks
{
    public class IndexModel : BasePageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        public List<TodoItem> Tasks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public TodoCategory? Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public SortOrder Sort { get; set; } = SortOrder.Newest;

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        //для пагинации
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public int TotalPages { get; set; }
        

        public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string GetPageUrl(int pageIndex)
        {
            return Url.Page("/Tasks/Index", null, new
            {
                pageIndex,
                category = Category,
                status = Status,
                sort = Sort,
                search = Search
            })!;
        }

        public string GetPageHtmxUrl(int pageIndex)
        {
            return Url.Page("/Tasks/Index", null, new
            {
                handler = "Page",
                pageIndex,
                category = Category,
                status = Status,
                sort = Sort,
                search = Search
            })!;
        }

        private IQueryable<TodoItem> BuildQuery()
        {
            var query = _context.TodoItems
                .Where(x => x.UserId == CurrentUserId);

            if (Category.HasValue)
            {
                query = query.Where(x =>
                    x.TodoCategory == Category.Value);
            }

            if (Status == "active")
            {
                query = query.Where(x => !x.IsCompleted);
            }
            else if (Status == "completed")
            {
                query = query.Where(x => x.IsCompleted);
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(x =>
                    EF.Functions.ILike(x.Title, $"%{Search}%") ||
                    EF.Functions.ILike(x.Description, $"%{Search}%"));
            }

            if (Sort == SortOrder.Oldest)
            {
                query = query.OrderBy(x => x.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }

            return query;
        }
        public async Task OnGetAsync()
        {
            _logger.LogInformation("Открытие страницы задач. Пользователь: {UserId}", CurrentUserId);

            if (PageIndex < 1)
            {
                PageIndex = 1;
            }

            var query = BuildQuery(); //получаем результат запроса с учетом фильтров и сортировки

            var totalItems = await query.CountAsync(); //получаем общее количество элементов для пагинации

            TotalPages = (int)Math.Ceiling(
                totalItems / (double)PageSize); //вычисляем общее количество страниц на основе общего количества элементов и размера страницы

            // если страница вышла за пределы (например, после поиска)
            if (PageIndex > TotalPages && TotalPages > 0)
            {
                PageIndex = TotalPages;
            }

            var skip = (PageIndex - 1) * PageSize; //вычисляем количество элементов, которые нужно пропустить для текущей страницы

            Tasks = await query
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync(); //получаем элементы для текущей страницы с учетом фильтров, сортировки и пагинации
        }

        //запрос для поиска
        public async Task<IActionResult> OnGetSearchAsync()
        {
            
            // 2. Защита от отрицательной страницы
            if (PageIndex < 1)
            {
                PageIndex = 1;
            }

            // 3. Строим запрос с фильтрами и сортировкой
            var query = BuildQuery();

            // 4. Подсчёт общего количества элементов
            var totalItems = await query.CountAsync();

            // 5. Вычисление общего количества страниц
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // если страница вышла за пределы (например, после поиска)
            if (PageIndex > TotalPages && TotalPages > 0)
            {
                PageIndex = TotalPages;
            }

            // 6. Пропуск элементов для текущей страницы
            var skip = (PageIndex - 1) * PageSize;

            // 7. Получение элементов для текущей страницы
            Tasks = await query
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();

            var isHtmx = Request.Headers["HX-Request"].FirstOrDefault() == "true";

            if (!isHtmx)
            {
                var url = Url.Page("/Tasks/Index", new
                {
                    pageIndex = PageIndex,
                    category = Category,
                    status = Status,
                    sort = Sort,
                    search = Search
                });
                return Redirect(url!);
            }

            // 8. Возвращаем Partial View с обновлённым списком
            return Partial("_TaskList", this);
        }

        public async Task<IActionResult> OnGetPageAsync()
        {

            if (PageIndex < 1)
            {
                PageIndex = 1;
            }

            var query = BuildQuery();

            var totalItems = await query.CountAsync();

            TotalPages = (int)Math.Ceiling(
                totalItems / (double)PageSize);

            if (TotalPages == 0)
            {
                PageIndex = 1;
            }
            else if (PageIndex > TotalPages)
            {
                PageIndex = TotalPages;
            }

            var skip = (PageIndex - 1) * PageSize;

            Tasks = await query
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();

            var isHtmx = Request.Headers["HX-Request"].FirstOrDefault() == "true";

            if (!isHtmx)
            {
                var url = Url.Page("/Tasks/Index", new
                {
                    pageIndex = PageIndex,
                    category = Category,
                    status = Status,
                    sort = Sort,
                    search = Search
                });
                return Redirect(url!);
            }

            return Partial("_TaskList", this);
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var todoItem = await _context.TodoItems
         .FirstOrDefaultAsync(x => x.Id == id &&
        x.UserId == CurrentUserId);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
