using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class RoleConfiguration : IEntityTypeConfiguration<Role> {
		public void Configure(EntityTypeBuilder<Role> builder) {
			builder.ToTable("roles");

			builder.HasKey(r => r.Id);

			builder.Property(r => r.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(r => r.Name)
				.HasColumnName("name")
				.HasMaxLength(25)
				.IsRequired();

			builder.Property(r => r.SortOrder)
				.HasColumnName("sort_order")
				.IsRequired();

			builder.Property(r => r.IsActive)
				.HasColumnName("is_active")
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(r => r.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(r => r.UpdatedAt)
				.HasColumnName("updated_at");

			builder.HasIndex(r => r.Name)
				.IsUnique();
		}
	}
}