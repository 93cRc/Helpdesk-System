namespace Helpdesk_System.ViewModels.Users {
	public class UserListItemViewModel {
		public int Id { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string? Phone { get; set; }

		public string RoleName { get; set; } = string.Empty;

		public string? DepartmentName { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime? LastLoginAt { get; set; }
	}
}