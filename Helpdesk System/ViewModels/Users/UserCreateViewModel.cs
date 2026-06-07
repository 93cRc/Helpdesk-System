using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helpdesk_System.ViewModels.Users {
	public class UserCreateViewModel {
		[Required(ErrorMessage = "Imię jest wymagane.")]
		[StringLength(50, ErrorMessage = "Imię może mieć maksymalnie 50 znaków.")]
		[Display(Name = "Imię")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Nazwisko jest wymagane.")]
		[StringLength(100, ErrorMessage = "Nazwisko może mieć maksymalnie 100 znaków.")]
		[Display(Name = "Nazwisko")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email jest wymagany.")]
		[EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
		[StringLength(255, ErrorMessage = "Email może mieć maksymalnie 255 znaków.")]
		[Display(Name = "Email")]
		public string Email { get; set; } = string.Empty;

		[StringLength(25, ErrorMessage = "Telefon może mieć maksymalnie 25 znaków.")]
		[Display(Name = "Telefon")]
		public string? Phone { get; set; }

		[Required(ErrorMessage = "Rola jest wymagana.")]
		[Display(Name = "Rola")]
		public int RoleId { get; set; }

		[Display(Name = "Dział")]
		public int? DepartmentId { get; set; }

		public List<SelectListItem> Roles { get; set; } = new();
		public List<SelectListItem> Departments { get; set; } = new();
	}
}