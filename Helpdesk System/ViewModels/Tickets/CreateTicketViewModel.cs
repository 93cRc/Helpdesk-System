using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helpdesk_System.ViewModels.Tickets
{
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "Kategoria jest wymagana.")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Priorytet jest wymagany.")]
        public int PriorityId { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Priorities { get; set; } = new();
    }
}