using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class CategoryConfiguration : IEntityTypeConfiguration<Category> {
		public void Configure(EntityTypeBuilder<Category> builder) {
			builder.ToTable("categories");

			builder.HasKey(c => c.Id);

			builder.Property(c => c.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(c => c.Name)
				.HasColumnName("name")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(c => c.SortOrder)
				.HasColumnName("sort_order")
				.IsRequired();

			builder.Property(c => c.IsActive)
				.HasColumnName("is_active")
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(c => c.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(c => c.UpdatedAt)
				.HasColumnName("updated_at");

			builder.HasIndex(c => c.Name)
				.IsUnique();

			builder.HasData(
				new Category {
					Id = 1,
					Name = "Hardware",
					SortOrder = 0,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Category {
					Id = 2,
					Name = "Software",
					SortOrder = 1,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Category {
					Id = 3,
					Name = "Konta i uprawnienia",
					SortOrder = 2,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Category {
					Id = 4,
					Name = "Sieć",
					SortOrder = 3,
					IsActive = true,
					CreatedAt = new DateTime()
				}
			);
		}
	}
}