using Helpdesk_System.Models.DbViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
/*
namespace Helpdesk_System.Data.Configurations {
	public class VTicketConfiguration : IEntityTypeConfiguration<VTicket> {
		public void Configure(EntityTypeBuilder<VTicket> builder) {
			builder.ToView("v_tickets");
			builder.HasNoKey();

			builder.Property(x => x.Id).HasColumnName("id");
			builder.Property(x => x.TicketTitle).HasColumnName("ticket_title").HasMaxLength(100);
			builder.Property(x => x.TicketDescription).HasColumnName("ticket_description");

			builder.Property(x => x.RequestorId).HasColumnName("requestor_id");
			builder.Property(x => x.RequestorFirstName).HasColumnName("requestor_first_name").HasMaxLength(50);
			builder.Property(x => x.RequestorLastName).HasColumnName("requestor_last_name").HasMaxLength(100);
			builder.Property(x => x.RequestorEmail).HasColumnName("requestor_email").HasMaxLength(255);

			builder.Property(x => x.DepartmentId).HasColumnName("department_id");
			builder.Property(x => x.DepartmentName).HasColumnName("department_name").HasMaxLength(50);

			builder.Property(x => x.AgentId).HasColumnName("agent_id");
			builder.Property(x => x.AgentFirstName).HasColumnName("agent_first_name").HasMaxLength(50);
			builder.Property(x => x.AgentLastName).HasColumnName("agent_last_name").HasMaxLength(100);
			builder.Property(x => x.AgentEmail).HasColumnName("agent_email").HasMaxLength(255);

			builder.Property(x => x.CategoryId).HasColumnName("category_id");
			builder.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(50);

			builder.Property(x => x.StatusId).HasColumnName("status_id");
			builder.Property(x => x.StatusCode).HasColumnName("status_code").HasMaxLength(50);
			builder.Property(x => x.StatusName).HasColumnName("status_name").HasMaxLength(50);

			builder.Property(x => x.PriorityId).HasColumnName("priority_id");
			builder.Property(x => x.PriorityCode).HasColumnName("priority_code").HasMaxLength(50);
			builder.Property(x => x.PriorityName).HasColumnName("priority_name").HasMaxLength(25);

			builder.Property(x => x.CreatedAt).HasColumnName("created_at");
			builder.Property(x => x.AssignedAt).HasColumnName("assigned_at");
			builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
			builder.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
			builder.Property(x => x.ClosedAt).HasColumnName("closed_at");
		}
	}
}*/