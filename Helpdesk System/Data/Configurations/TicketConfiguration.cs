using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk_System.Data.Configurations {
	public class TicketConfiguration : IEntityTypeConfiguration<Ticket> {
		public void Configure(EntityTypeBuilder<Ticket> builder) {
			builder.ToTable("tickets");

			builder.HasKey(t => t.Id);

			builder.Property(t => t.Id)
				.HasColumnName("id")
				.ValueGeneratedOnAdd();

			builder.Property(t => t.RequestorId)
				.HasColumnName("requestor_id")
				.IsRequired();

			builder.Property(t => t.DepartmentId)
				.HasColumnName("department_id");

			builder.Property(t => t.AgentId)
				.HasColumnName("agent_id");

			builder.Property(t => t.Title)
				.HasColumnName("title")
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(t => t.Description)
				.HasColumnName("description");
				//.HasColumnType("text");

			builder.Property(t => t.CategoryId)
				.HasColumnName("category_id");

			builder.Property(t => t.StatusId)
				.HasColumnName("status_id")
				.IsRequired();

			builder.Property(t => t.PriorityId)
				.HasColumnName("priority_id")
				.IsRequired();

			builder.Property(t => t.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(t => t.AssignedAt)
				.HasColumnName("assigned_at");

			builder.Property(t => t.UpdatedAt)
				.HasColumnName("updated_at");

			builder.Property(t => t.ResolvedAt)
				.HasColumnName("resolved_at");

			builder.Property(t => t.ClosedAt)
				.HasColumnName("closed_at");

			builder.HasOne(t => t.Requestor)
				.WithMany(u => u.RequestedTickets)
				.HasForeignKey(t => t.RequestorId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(t => t.Agent)
				.WithMany(u => u.AssignedTickets)
				.HasForeignKey(t => t.AgentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(t => t.Department)
				.WithMany(d => d.Tickets)
				.HasForeignKey(t => t.DepartmentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(t => t.Category)
				.WithMany(c => c.Tickets)
				.HasForeignKey(t => t.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(t => t.Status)
				.WithMany(s => s.Tickets)
				.HasForeignKey(t => t.StatusId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(t => t.Priority)
				.WithMany(p => p.Tickets)
				.HasForeignKey(t => t.PriorityId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(t => t.RequestorId);
			builder.HasIndex(t => t.AgentId);
			builder.HasIndex(t => t.DepartmentId);
			builder.HasIndex(t => t.CategoryId);
			builder.HasIndex(t => t.StatusId);
			builder.HasIndex(t => t.PriorityId);
			builder.HasIndex(t => t.CreatedAt);

			builder.HasIndex(t => new { t.StatusId, t.AgentId, t.CreatedAt });

			builder.HasIndex(t => new { t.RequestorId, t.CreatedAt });

			builder.HasIndex(t => new { t.DepartmentId, t.StatusId, t.CreatedAt });
		}
	}
}