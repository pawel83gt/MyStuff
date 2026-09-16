using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor.Pages;

public abstract class BasePageModel : PageModel
{
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}