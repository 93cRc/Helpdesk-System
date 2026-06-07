using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class PriorityConfiguration : IEntityTypeConfiguration<Priority> {
		public void Configure(EntityTypeBuilder<Priority> builder) {
			builder.ToTable("priorities");

			builder.HasKey(p => p.Id);

			builder.Property(p => p.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(p => p.Code)
				.HasColumnName("code")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(p => p.Name)
				.HasColumnName("name")
				.HasMaxLength(25)
				.IsRequired();

			builder.Property(p => p.SortOrder)
				.HasColumnName("sort_order")
				.IsRequired();

			builder.Property(p => p.IsActive)
				.HasColumnName("is_active")
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(p => p.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(p => p.UpdatedAt)
				.HasColumnName("updated_at");

			builder.HasIndex(p => p.Code)
				.IsUnique();

			builder.HasIndex(p => p.Name)
				.IsUnique();

			builder.HasData(
				new Priority {
					Id = 1,
					Code = "none",
					Name = "Brak",
					SortOrder = 0,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Priority {
					Id = 2,
					Code = "low",
					Name = "Niski",
					SortOrder = 1,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Priority {
					Id = 3,
					Code = "medium",
					Name = "Średni",
					SortOrder = 2,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Priority {
					Id = 4,
					Code = "high",
					Name = "Wysoki",
					SortOrder = 3,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Priority {
					Id = 5,
					Code = "critical",
					Name = "Krytyczny",
					SortOrder = 4,
					IsActive = true,
					CreatedAt = new DateTime()
				}
			);
		}
	}
}