using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class UserConfiguration : IEntityTypeConfiguration<User> {
		public void Configure(EntityTypeBuilder<User> builder) {
			builder.ToTable("users");

			builder.HasKey(u => u.Id);

			builder.Property(u => u.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(u => u.FirstName)
				.HasColumnName("first_name")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(u => u.LastName)
				.HasColumnName("last_name")
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(u => u.Email)
				.HasColumnName("email")
				.HasMaxLength(255)
				.IsRequired();

			builder.Property(u => u.Phone)
				.HasColumnName("phone")
				.HasMaxLength(25);

			builder.Property(u => u.PasswordHash)
				.HasColumnName("password_hash")
				.HasMaxLength(255)
				.IsRequired();

			builder.Property(u => u.RoleId)
				.HasColumnName("role_id")
				.IsRequired();

			builder.Property(u => u.DepartmentId)
				.HasColumnName("department_id");

			builder.Property(u => u.IsActive)
				.HasColumnName("is_active")
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(u => u.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(u => u.UpdatedAt)
				.HasColumnName("updated_at");

			builder.Property(u => u.LastLoginAt)
				.HasColumnName("last_login_at");

			builder.HasIndex(u => u.Email)
				.IsUnique();

			builder.HasOne(u => u.Role)
				.WithMany(r => r.Users)
				.HasForeignKey(u => u.RoleId);

			builder.HasOne(u => u.Department)
				.WithMany(d => d.Users)
				.HasForeignKey(u => u.DepartmentId);

			builder.HasData(
				new User {
					Id = 1,
					FirstName = "Kamil",
					LastName = "Krakowiak",
					Email = "admin@firma.pl",
					Phone = null,
					PasswordHash = "$2b$12$LxOxtse..3EaSbWJqFHlRerxBudpeHpvgA5fhRamMO.PbFqWS6UJu",
					RoleId = 1,
					DepartmentId = null,
					IsActive = true,
					CreatedAt = new(),
					UpdatedAt = null,
					LastLoginAt = null
				}
			);
		}
	}
}