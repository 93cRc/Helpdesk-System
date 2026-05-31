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
		}
	}
}