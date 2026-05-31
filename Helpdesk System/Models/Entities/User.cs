namespace Helpdesk_System.Models.Entities {
	public class User {
		public int Id { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string? Phone { get; set; }

		public string PasswordHash { get; set; } = string.Empty;

		public int RoleId { get; set; }

		public int? DepartmentId { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? LastLoginAt { get; set; }

		public Role? Role { get; set; }

		public Department? Department { get; set; }

		public ICollection<Ticket> RequestedTickets { get; set; } = new List<Ticket>();

		public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

		public ICollection<Comment> Comments { get; set; } = new List<Comment>();

		public ICollection<TicketRating> TicketRatings { get; set; } = new List<TicketRating>();
	}
}