using Helpdesk_System.Services.Interfaces;

namespace Helpdesk_System.Services {
	public class PasswordHasherService : IPasswordHasherService {
		private const int CostFactor = 12;

		public string HashPassword(string password) {
			if (string.IsNullOrWhiteSpace(password)) {
				throw new ArgumentException("Hasło nie może być puste.", nameof(password));
			}

			return BCrypt.Net.BCrypt.HashPassword(password, CostFactor);
		}

		public bool VerifyPassword(string password, string passwordHash) {
			if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash)) {
				return false;
			}

			return BCrypt.Net.BCrypt.Verify(password, passwordHash);
		}
	}
}