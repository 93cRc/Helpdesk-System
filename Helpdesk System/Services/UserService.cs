using System.Security.Cryptography;
using BCrypt.Net;
using Helpdesk_System.Data;
using Helpdesk_System.Models.Entities;
using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_System.Services {
	public class UserService : IUserService {
		private readonly HelpdeskSystemDbContext _context;

		public UserService(HelpdeskSystemDbContext context) {
			_context = context;
		}

		public async Task<UserIndexViewModel> GetIndexViewModelAsync(int? roleId, int? departmentId, bool? isActive) {
			var query = _context.Users
				.Include(u => u.Role)
				.Include(u => u.Department)
				.AsQueryable();

			if (roleId.HasValue) {
				query = query.Where(u => u.RoleId == roleId.Value);
			}

			if (departmentId.HasValue) {
				query = query.Where(u => u.DepartmentId == departmentId.Value);
			}

			if (isActive.HasValue) {
				query = query.Where(u => u.IsActive == isActive.Value);
			}

			var users = await query
				.OrderBy(u => u.LastName)
				.ThenBy(u => u.FirstName)
				.Select(u => new UserListItemViewModel {
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Email = u.Email,
					Phone = u.Phone,
					RoleName = u.Role.Name,
					DepartmentName = u.Department != null ? u.Department.Name : null,
					IsActive = u.IsActive,
					LastLoginAt = u.LastLoginAt,
					CreatedAt = u.CreatedAt
				})
				.ToListAsync();

			var roles = await _context.Roles
				.Where(r => r.IsActive)
				.OrderBy(r => r.SortOrder)
				.ThenBy(r => r.Name)
				.Select(r => new SelectListItem {
					Value = r.Id.ToString(),
					Text = r.Name
				})
				.ToListAsync();

			var departments = await _context.Departments
				.Where(d => d.IsActive)
				.OrderBy(d => d.SortOrder)
				.ThenBy(d => d.Name)
				.Select(d => new SelectListItem {
					Value = d.Id.ToString(),
					Text = d.Name
				})
				.ToListAsync();

			return new UserIndexViewModel {
				Users = users,
				Roles = roles,
				Departments = departments
			};
		}

		public async Task<UserCreateViewModel> GetCreateViewModelAsync() {
			return new UserCreateViewModel {
				Roles = await GetRoleSelectListAsync(null),
				Departments = await GetDepartmentSelectListAsync(null)
			};
		}

		public async Task<CreateUserResultViewModel> CreateAsync(UserCreateViewModel model) {
			var generatedPassword = GeneratePassword();

			var user = new User {
				FirstName = model.FirstName.Trim(),
				LastName = model.LastName.Trim(),
				Email = model.Email.Trim().ToLower(),
				Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
				RoleId = model.RoleId,
				DepartmentId = model.DepartmentId,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
				IsActive = true,
				CreatedAt = DateTime.Now,
				UpdatedAt = null,
				LastLoginAt = null
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return new CreateUserResultViewModel {
				UserId = user.Id,
				FullName = $"{user.FirstName} {user.LastName}",
				Email = user.Email,
				GeneratedPassword = generatedPassword
			};
		}

		public async Task<UserEditViewModel?> GetEditViewModelAsync(int id) {
			var user = await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(user => user.Id == id);

			if (user == null) {
				return null;
			}

			return new UserEditViewModel {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Phone = user.Phone,
				RoleId = user.RoleId,
				DepartmentId = user.DepartmentId,
				IsActive = user.IsActive,
				Roles = await GetRoleSelectListAsync(user.RoleId),
				Departments = await GetDepartmentSelectListAsync(user.DepartmentId)
			};
		}

		public async Task<bool> UpdateAsync(UserEditViewModel model) {
			var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == model.Id);

			if (user == null) {
				return false;
			}

			user.FirstName = model.FirstName.Trim();
			user.LastName = model.LastName.Trim();
			user.Email = model.Email.Trim().ToLower();
			user.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
			user.RoleId = model.RoleId;
			user.DepartmentId = model.DepartmentId;
			user.IsActive = model.IsActive;
			user.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<UserDetailsViewModel?> GetDetailsAsync(int id) {
			return await _context.Users
				.Include(user => user.Role)
				.Include(user => user.Department)
				.Where(user => user.Id == id)
				.Select(user => new UserDetailsViewModel {
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					Phone = user.Phone,
					RoleName = user.Role.Name,
					DepartmentName = user.Department == null ? null : user.Department.Name,
					IsActive = user.IsActive,
					CreatedAt = user.CreatedAt,
					UpdatedAt = user.UpdatedAt,
					LastLoginAt = user.LastLoginAt
				})
				.FirstOrDefaultAsync();
		}

		public async Task<UserDeactivateViewModel?> GetDeactivateViewModelAsync(int id) {
			return await _context.Users
				.Include(user => user.Role)
				.Include(user => user.Department)
				.Where(user => user.Id == id)
				.Select(user => new UserDeactivateViewModel {
					Id = user.Id,
					FullName = user.FirstName + " " + user.LastName,
					Email = user.Email,
					RoleName = user.Role.Name,
					DepartmentName = user.Department == null ? null : user.Department.Name,
					IsActive = user.IsActive
				})
				.FirstOrDefaultAsync();
		}

		public async Task<bool> DeactivateAsync(int id) {
			var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

			if (user == null) {
				return false;
			}

			user.IsActive = false;
			user.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ActivateAsync(int id) {
			var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

			if (user == null) {
				return false;
			}

			user.IsActive = true;
			user.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<ResetPasswordResultViewModel?> ResetPasswordAsync(int id) {
			var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

			if (user == null) {
				return null;
			}

			var generatedPassword = GeneratePassword();

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword);
			user.LastLoginAt = null;
			user.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();

			return new ResetPasswordResultViewModel {
				UserId = user.Id,
				FullName = $"{user.FirstName} {user.LastName}",
				Email = user.Email,
				GeneratedPassword = generatedPassword
			};
		}

		public async Task<bool> EmailExistsAsync(string email, int? excludedUserId = null) {
			var normalizedEmail = email.Trim().ToLower();

			return await _context.Users.AnyAsync(user =>
				user.Email == normalizedEmail &&
				(!excludedUserId.HasValue || user.Id != excludedUserId.Value));
		}

		private async Task<List<SelectListItem>> GetRoleSelectListAsync(int? selectedRoleId) {
			return await _context.Roles
				.Where(role => role.IsActive)
				.OrderBy(role => role.SortOrder)
				.ThenBy(role => role.Name)
				.Select(role => new SelectListItem {
					Value = role.Id.ToString(),
					Text = role.Name,
					Selected = selectedRoleId.HasValue && role.Id == selectedRoleId.Value
				})
				.ToListAsync();
		}

		private async Task<List<SelectListItem>> GetDepartmentSelectListAsync(int? selectedDepartmentId) {
			var departments = await _context.Departments
				.Where(department => department.IsActive)
				.OrderBy(department => department.SortOrder)
				.ThenBy(department => department.Name)
				.Select(department => new SelectListItem {
					Value = department.Id.ToString(),
					Text = department.Name,
					Selected = selectedDepartmentId.HasValue && department.Id == selectedDepartmentId.Value
				})
				.ToListAsync();

			departments.Insert(0, new SelectListItem {
				Value = string.Empty,
				//Text = "< brak działu >",
				Selected = !selectedDepartmentId.HasValue
			});

			return departments;
		}

		private static string GeneratePassword() {
			int length = 12;
			string lowercase = "abcdefghijkmnopqrstuvwxyz";
			string uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
			string digits = "23456789";
			string special = "!@$?_-";
			string allCharacters = lowercase + uppercase + digits + special;

			var passwordCharacters = new List<char>
			{
				GetRandomCharacter(lowercase),
				GetRandomCharacter(uppercase),
				GetRandomCharacter(digits),
				GetRandomCharacter(special)
			};

			while (passwordCharacters.Count < length) {
				passwordCharacters.Add(GetRandomCharacter(allCharacters));
			}

			return new string(passwordCharacters
				.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue))
				.ToArray());
		}

		private static char GetRandomCharacter(string characters) {
			return characters[RandomNumberGenerator.GetInt32(characters.Length)];
		}
	}
}