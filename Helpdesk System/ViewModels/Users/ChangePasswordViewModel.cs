using System.ComponentModel.DataAnnotations;

namespace Helpdesk_System.ViewModels.Accounts {
	public class ChangePasswordViewModel {
		[Required(ErrorMessage = "Aktualne hasło jest wymagane.")]
		[DataType(DataType.Password)]
		[Display(Name = "Aktualne hasło")]
		public string CurrentPassword { get; set; } = string.Empty;

		[Required(ErrorMessage = "Nowe hasło jest wymagane.")]
		[DataType(DataType.Password)]
		[Display(Name = "Nowe hasło")]
		[MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
		public string NewPassword { get; set; } = string.Empty;

		[Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
		[DataType(DataType.Password)]
		[Display(Name = "Powtórz nowe hasło")]
		[Compare(
			nameof(NewPassword),
			ErrorMessage = "Podane hasła nie są takie same.")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}