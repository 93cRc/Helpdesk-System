namespace Helpdesk_System.ViewModels.Users {
	public class UserDeactivateViewModel {
		public int Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string RoleName { get; set; } = string.Empty;
		public string? DepartmentName { get; set; }
		public bool IsActive { get; set; }
	}
}