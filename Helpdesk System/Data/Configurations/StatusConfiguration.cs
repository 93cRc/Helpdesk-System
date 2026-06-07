using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class StatusConfiguration : IEntityTypeConfiguration<Status> {
		public void Configure(EntityTypeBuilder<Status> builder) {
			builder.ToTable("statuses");

			builder.HasKey(s => s.Id);

			builder.Property(s => s.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(s => s.Code)
				.HasColumnName("code")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(s => s.Name)
				.HasColumnName("name")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(s => s.SortOrder)
				.HasColumnName("sort_order")
				.IsRequired();

			builder.Property(s => s.IsActive)
				.HasColumnName("is_active")
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(s => s.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(s => s.UpdatedAt)
				.HasColumnName("updated_at");

			builder.HasIndex(s => s.Code)
				.IsUnique();

			builder.HasIndex(s => s.Name)
				.IsUnique();

			builder.HasData(
				new Status {
					Id = 1,
					Code = "new",
					Name = "Nowe",
					SortOrder = 0,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Status {
					Id = 2,
					Code = "in_progress",
					Name = "W toku",
					SortOrder = 1,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Status {
					Id = 3,
					Code = "waiting_for_requestor",
					Name = "Oczekuje na zgłaszającego",
					SortOrder = 2,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Status {
					Id = 4,
					Code = "waiting_for_team",
					Name = "Oczekuje na zespół",
					SortOrder = 3,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Status {
					Id = 5,
					Code = "on_hold",
					Name = "Wstrzymane",
					SortOrder = 4,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Status {
					Id = 6,
					Code = "resolved",
					Name = "Rozwiązane",
					SortOrder = 5,
					IsActive = true,
					CreatedAt = new DateTime()
				},
				new Status {
					Id = 7,
					Code = "closed",
					Name = "Zamknięte",
					SortOrder = 6,
					IsActive = true,
					CreatedAt = new DateTime()
				}
			);
		}
	}
}