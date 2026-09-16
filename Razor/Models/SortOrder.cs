using System.ComponentModel.DataAnnotations;

namespace Razor.Models
{
    public enum SortOrder
    {
        [Display(Name = "Сначала новые")]
        Newest,

        [Display(Name = "Сначала старые")]
        Oldest
    }
}
