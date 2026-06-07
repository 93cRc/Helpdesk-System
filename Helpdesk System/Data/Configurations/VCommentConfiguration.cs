using Helpdesk_System.Models.DbViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
/*
namespace Helpdesk_System.Data.Configurations {
	public class VCommentConfiguration : IEntityTypeConfiguration<VComment> {
		public void Configure(EntityTypeBuilder<VComment> builder) {
			builder.ToView("v_comments");
			builder.HasNoKey();

			builder.Property(x => x.Id).HasColumnName("id");
			builder.Property(x => x.TicketId).HasColumnName("ticket_id");
			builder.Property(x => x.UserId).HasColumnName("user_id");
			builder.Property(x => x.UserFirstName).HasColumnName("user_first_name").HasMaxLength(50);
			builder.Property(x => x.UserLastName).HasColumnName("user_last_name").HasMaxLength(100);
			builder.Property(x => x.UserEmail).HasColumnName("user_email").HasMaxLength(255);
			builder.Property(x => x.Content).HasColumnName("content");
			builder.Property(x => x.IsInternal).HasColumnName("is_internal");
			builder.Property(x => x.CreatedAt).HasColumnName("created_at");
			builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
		}
	}
}*/