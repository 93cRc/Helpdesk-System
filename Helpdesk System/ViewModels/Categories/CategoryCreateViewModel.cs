using System.ComponentModel.DataAnnotations;

namespace Helpdesk_System.ViewModels.Categories
{
    public class CategoryCreateViewModel
    {
        [Required]
        [Display(Name = "Nazwa kategorii")]
        public string Name { get; set; } = null!;
    }
}