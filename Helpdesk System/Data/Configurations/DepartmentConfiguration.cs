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
			
			var createdAt = new DateTime();

			builder.HasData(
				new Department {
					Id = 1,
					Name = "IT - Dev",
					SortOrder = 0,
					IsActive = true,
					CreatedAt = createdAt
				},
				new Department {
					Id = 2,
					Name = "IT - Support",
					SortOrder = 1,
					IsActive = true,
					CreatedAt = createdAt
				},
				new Department {
					Id = 3,
					Name = "IT - Infrastructure",
					SortOrder = 2,
					IsActive = true,
					CreatedAt = createdAt
				},
				new Department {
					Id = 4,
					Name = "Produkcja",
					SortOrder = 3,
					IsActive = true,
					CreatedAt = createdAt
				},
				new Department {
					Id = 5,
					Name = "Księgowość",
					SortOrder = 4,
					IsActive = true,
					CreatedAt = createdAt
				}
			);
		}
	}
}