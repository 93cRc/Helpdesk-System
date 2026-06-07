using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helpdesk_System.ViewModels.Users {
	public class UserIndexViewModel {
		public List<UserListItemViewModel> Users { get; set; } = new();
		public List<SelectListItem> Roles { get; set; } = new();
		public List<SelectListItem> Departments { get; set; } = new();
	}
}