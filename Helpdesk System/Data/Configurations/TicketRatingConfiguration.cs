using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class TicketRatingConfiguration : IEntityTypeConfiguration<TicketRating> {
		public void Configure(EntityTypeBuilder<TicketRating> builder) {
			builder.ToTable("ticket_ratings");

			builder.HasKey(r => r.Id);

			builder.Property(r => r.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(r => r.TicketId)
				.HasColumnName("ticket_id")
				.IsRequired();

			builder.Property(r => r.UserId)
				.HasColumnName("user_id")
				.IsRequired();

			builder.Property(r => r.Rating)
				.HasColumnName("rating")
				.IsRequired();

			builder.Property(r => r.Content)
				.HasColumnName("content")
				.HasColumnType("text");

			builder.Property(r => r.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.HasOne(r => r.Ticket)
				.WithMany(t => t.Ratings)
				.HasForeignKey(r => r.TicketId);

			builder.HasOne(r => r.User)
				.WithMany(u => u.TicketRatings)
				.HasForeignKey(r => r.UserId);

			builder.ToTable(t => {
				t.HasCheckConstraint("chk_ticket_ratings_rating", "rating between 1 and 5");
			});
		}
	}
}