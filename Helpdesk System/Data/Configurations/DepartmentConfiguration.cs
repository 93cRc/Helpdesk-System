using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class DepartmentConfiguration : IEntityTypeConfiguration<Department> {
		public void Configure(EntityTypeBuilder<Department> builder) {
			builder.ToTable("departments");

			builder.HasKey(d => d.Id);

			builder.Property(d => d.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(d => d.Name)
				.HasColumnName("name")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(d => d.SortOrder)
				.HasColumnName("sort_order")
				.IsRequired();

			builder.Property(d => d.IsActive)
				.HasColumnName("is_active")
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(d => d.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(d => d.UpdatedAt)
				.HasColumnName("updated_at");

			builder.HasIndex(d => d.Name)
				.IsUnique();
		}
	}
}