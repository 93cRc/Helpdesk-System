using System.ComponentModel.DataAnnotations;

namespace Helpdesk_System.ViewModels.Categories
{
    public class CategoryEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nazwa kategorii")]
        public string Name { get; set; } = null!;
    }
}