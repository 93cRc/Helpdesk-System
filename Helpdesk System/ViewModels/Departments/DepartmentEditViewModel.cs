using System.ComponentModel.DataAnnotations;

namespace Helpdesk_System.ViewModels.Departments
{
    public class DepartmentEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa działu jest wymagana.")]
        [StringLength(100)]
        public string Name { get; set; } = null!;
    }
}