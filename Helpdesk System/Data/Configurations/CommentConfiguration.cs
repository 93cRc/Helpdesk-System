using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
		public void Configure(EntityTypeBuilder<Comment> builder) {
			builder.ToTable("comments");

			builder.HasKey(c => c.Id);

			builder.Property(c => c.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(c => c.TicketId)
				.HasColumnName("ticket_id")
				.IsRequired();

			builder.Property(c => c.UserId)
				.HasColumnName("user_id")
				.IsRequired();

			builder.Property(c => c.Content)
				.HasColumnName("content")
				.HasColumnType("text")
				.IsRequired();

			builder.Property(c => c.IsInternal)
				.HasColumnName("is_internal")
				.IsRequired()
				.HasDefaultValue(false);

			builder.Property(c => c.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(c => c.UpdatedAt)
				.HasColumnName("updated_at");

			builder.Property(c => c.DeletedAt)
				.HasColumnName("deleted_at");

			builder.HasOne(c => c.Ticket)
				.WithMany(t => t.Comments)
				.HasForeignKey(c => c.TicketId);

			builder.HasOne(c => c.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId);

			builder.HasIndex(c => c.TicketId);
			builder.HasIndex(c => c.UserId);
		}
	}
}