using System.ComponentModel.DataAnnotations;

namespace Razor.Models
{
    public enum TodoCategory
    {
        [Display(Name = "Работа")]
        Work,

        [Display(Name = "Дом")]
        Home,

        [Display(Name = "Учёба")]
        Study,

        [Display(Name = "Развлечения")]
        Entertainment,

        [Display(Name = "Другое")]
        Other
    }
}
