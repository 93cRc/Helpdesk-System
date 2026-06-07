namespace Helpdesk_System.ViewModels.Users {
	public class CreateUserResultViewModel {
		public int UserId { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string GeneratedPassword { get; set; } = string.Empty;
	}
}