using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller {
		private readonly IUserService _userService;

		public UsersController(IUserService userService) {
			_userService = userService;
		}

		public async Task<IActionResult> Index(int? roleId, int? departmentId, bool? isActive) {
			var model = await _userService.GetIndexViewModelAsync(roleId, departmentId, isActive);
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id) {
			var viewModel = await _userService.GetDetailsAsync(id);

			if (viewModel == null) {
				return NotFound();
			}

			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> Create() {
			var viewModel = await _userService.GetCreateViewModelAsync();
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserCreateViewModel viewModel) {
			if (await _userService.EmailExistsAsync(viewModel.Email)) {
				ModelState.AddModelError(nameof(viewModel.Email), "Użytkownik z takim adresem email już istnieje.");
			}

			if (!ModelState.IsValid) {
				var refreshedViewModel = await _userService.GetCreateViewModelAsync();
				viewModel.Roles = refreshedViewModel.Roles;
				viewModel.Departments = refreshedViewModel.Departments;

				return View(viewModel);
			}

			var result = await _userService.CreateAsync(viewModel);
			return View("GeneratedPassword", result);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id) {
			var viewModel = await _userService.GetEditViewModelAsync(id);

			if (viewModel == null) {
				return NotFound();
			}

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(UserEditViewModel viewModel) {
			if (await _userService.EmailExistsAsync(viewModel.Email, viewModel.Id)) {
				ModelState.AddModelError(nameof(viewModel.Email), "Użytkownik z takim adresem email już istnieje.");
			}

			if (!ModelState.IsValid) {
				var refreshedViewModel = await _userService.GetEditViewModelAsync(viewModel.Id);

				if (refreshedViewModel == null) {
					return NotFound();
				}

				viewModel.Roles = refreshedViewModel.Roles;
				viewModel.Departments = refreshedViewModel.Departments;

				return View(viewModel);
			}

			var updated = await _userService.UpdateAsync(viewModel);

			if (!updated) {
				return NotFound();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Deactivate(int id) {
			var viewModel = await _userService.GetDeactivateViewModelAsync(id);

			if (viewModel == null) {
				return NotFound();
			}

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeactivateConfirmed(int id) {
			var deactivated = await _userService.DeactivateAsync(id);

			if (!deactivated) {
				return NotFound();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Activate(int id) {
			var activated = await _userService.ActivateAsync(id);

			if (!activated) {
				return NotFound();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(int id) {
			var result = await _userService.ResetPasswordAsync(id);

			if (result == null) {
				return NotFound();
			}

			return View("ResetPasswordResult", result);
		}
	}
}