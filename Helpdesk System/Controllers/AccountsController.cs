using Helpdesk_System.Data;
using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Helpdesk_System.Controllers {
	public class AccountsController : Controller {
		private readonly HelpdeskSystemDbContext _context;
		private readonly IPasswordHasherService _passwordHasherService;

		public AccountsController(HelpdeskSystemDbContext context, IPasswordHasherService passwordHasherService) {
			_context = context;
			_passwordHasherService = passwordHasherService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login() {
			if (User.Identity?.IsAuthenticated == true) {
				return RedirectToAction("Index", "Tickets");
			}

			return View(new LoginViewModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model) {
			if (!ModelState.IsValid) {
				return View(model);
			}

			var user = await _context.Users
				.Include(x => x.Role)
				.Include(x => x.Department)
				.FirstOrDefaultAsync(x =>
					x.Email == model.Email &&
					x.IsActive);

			if (user == null || !_passwordHasherService.VerifyPassword(model.Password, user.PasswordHash)) {
				ModelState.AddModelError(
					string.Empty,
					"Nieprawidłowy e-mail lub hasło.");

				return View(model);
			}

			bool mustChangePassword = user.LastLoginAt == null;

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
				new Claim(ClaimTypes.Role, user.Role.Name),
				new Claim("FirstName", user.FirstName),
				new Claim("LastName", user.LastName),
				new Claim("MustChangePassword", mustChangePassword.ToString())
			};

			if (user.DepartmentId.HasValue) {
				claims.Add(new Claim("DepartmentId", user.DepartmentId.Value.ToString()));
			}

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			var authProperties = new AuthenticationProperties {
				IsPersistent = model.RememberMe,
				ExpiresUtc = model.RememberMe
					? DateTimeOffset.UtcNow.AddDays(14)
					: DateTimeOffset.UtcNow.AddHours(8)
			};

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

			if (mustChangePassword) {
				return RedirectToAction(nameof(ChangePassword));
			}

			return RedirectToAction("Index", "Tickets");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout() {
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction(nameof(Login));
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied() {
			return View();
		}

		[HttpGet]
		[Authorize]
		public IActionResult ChangePassword() {
			return View(new ChangePasswordViewModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
			if (!ModelState.IsValid) {
				return View(model);
			}

			var userIdValue = User.FindFirstValue(
				ClaimTypes.NameIdentifier);

			if (!int.TryParse(userIdValue, out int userId)) {
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

				return RedirectToAction(nameof(Login));
			}

			var user = await _context.Users
				.FirstOrDefaultAsync(x =>
					x.Id == userId &&
					x.IsActive);

			if (user == null) {
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

				return RedirectToAction(nameof(Login));
			}

			if (!_passwordHasherService.VerifyPassword(model.CurrentPassword, user.PasswordHash)) {
				ModelState.AddModelError(
					nameof(model.CurrentPassword),
					"Aktualne hasło jest nieprawidłowe.");

				return View(model);
			}

			if (_passwordHasherService.VerifyPassword(model.NewPassword, user.PasswordHash)) {
				ModelState.AddModelError(
					nameof(model.NewPassword),
					"Nowe hasło musi być inne niż aktualne hasło.");

				return View(model);
			}

			user.PasswordHash =
				_passwordHasherService.HashPassword(
					model.NewPassword);

			user.LastLoginAt = DateTime.Now;
			user.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();

			await HttpContext.SignOutAsync(
				CookieAuthenticationDefaults.AuthenticationScheme);

			TempData["SuccessMessage"] =
				"Hasło zostało zmienione. Zaloguj się ponownie nowym hasłem.";

			return RedirectToAction(nameof(Login));
		}
	}
}