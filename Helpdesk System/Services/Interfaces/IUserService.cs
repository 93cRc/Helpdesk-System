using Helpdesk_System.ViewModels.Users;

namespace Helpdesk_System.Services.Interfaces {
	public interface IUserService {
		Task<UserIndexViewModel> GetIndexViewModelAsync(int? roleId, int? departmentId, bool? isActive);
		Task<UserCreateViewModel> GetCreateViewModelAsync();
		Task<CreateUserResultViewModel> CreateAsync(UserCreateViewModel model);
		Task<UserEditViewModel?> GetEditViewModelAsync(int id);
		Task<bool> UpdateAsync(UserEditViewModel model);
		Task<UserDetailsViewModel?> GetDetailsAsync(int id);
		Task<UserDeactivateViewModel?> GetDeactivateViewModelAsync(int id);
		Task<bool> DeactivateAsync(int id);
		Task<bool> ActivateAsync(int id);
		Task<ResetPasswordResultViewModel?> ResetPasswordAsync(int id);
		Task<bool> EmailExistsAsync(string email, int? excludedUserId = null);
	}
}
