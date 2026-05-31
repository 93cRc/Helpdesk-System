using Helpdesk_System.Models.DbViews;
using Helpdesk_System.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_System.Data {
	public class HelpdeskSystemDbContext : DbContext {
		public HelpdeskSystemDbContext(DbContextOptions<HelpdeskSystemDbContext> options)
			: base(options) {
		}

		public DbSet<Role> Roles { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Status> Statuses { get; set; }
		public DbSet<Priority> Priorities { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<TicketRating> TicketRatings { get; set; }
		public DbSet<TicketHistory> TicketsHistory { get; set; }

		public DbSet<VTicket> VTickets { get; set; }
		public DbSet<VComment> VComments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(HelpdeskSystemDbContext).Assembly);
		}
	}
}