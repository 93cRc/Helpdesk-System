using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory> {
		public void Configure(EntityTypeBuilder<TicketHistory> builder) {
			builder.ToTable("tickets_history");

			builder.HasKey(h => h.Id);

			builder.Property(h => h.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(h => h.TicketId)
				.HasColumnName("ticket_id")
				.IsRequired();

			builder.Property(h => h.RequestorId)
				.HasColumnName("requestor_id")
				.IsRequired();

			builder.Property(h => h.DepartmentId)
				.HasColumnName("department_id");

			builder.Property(h => h.AgentId)
				.HasColumnName("agent_id");

			builder.Property(h => h.Title)
				.HasColumnName("title")
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(h => h.Description)
				.HasColumnName("description")
				.HasColumnType("text");

			builder.Property(h => h.CategoryId)
				.HasColumnName("category_id");

			builder.Property(h => h.StatusId)
				.HasColumnName("status_id")
				.IsRequired();

			builder.Property(h => h.PriorityId)
				.HasColumnName("priority_id")
				.IsRequired();

			builder.Property(h => h.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(h => h.AssignedAt)
				.HasColumnName("assigned_at");

			builder.Property(h => h.UpdatedAt)
				.HasColumnName("updated_at");

			builder.Property(h => h.ResolvedAt)
				.HasColumnName("resolved_at");

			builder.Property(h => h.ClosedAt)
				.HasColumnName("closed_at");

			builder.Property(h => h.ChangedBy)
				.HasColumnName("changed_by");

			builder.Property(h => h.HistoryCreatedAt)
				.HasColumnName("history_created_at")
				.IsRequired();

			builder.HasOne(h => h.Ticket)
				.WithMany(t => t.History)
				.HasForeignKey(h => h.TicketId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.Requestor)
				.WithMany()
				.HasForeignKey(h => h.RequestorId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.Agent)
				.WithMany()
				.HasForeignKey(h => h.AgentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.Department)
				.WithMany()
				.HasForeignKey(h => h.DepartmentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.Category)
				.WithMany()
				.HasForeignKey(h => h.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.Status)
				.WithMany()
				.HasForeignKey(h => h.StatusId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.Priority)
				.WithMany()
				.HasForeignKey(h => h.PriorityId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(h => h.ChangedByUser)
				.WithMany()
				.HasForeignKey(h => h.ChangedBy)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(h => h.TicketId);
			builder.HasIndex(h => h.HistoryCreatedAt);
		}
	}
}