namespace Helpdesk_System.Models.Entities {
	public class Ticket {
		public int Id { get; set; }

		public int RequestorId { get; set; }

		public int? DepartmentId { get; set; }

		public int? AgentId { get; set; }

		public string Title { get; set; } = string.Empty;

		public string? Description { get; set; }

		public int? CategoryId { get; set; }

		public int StatusId { get; set; }

		public int PriorityId { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime? AssignedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? ResolvedAt { get; set; }

		public DateTime? ClosedAt { get; set; }

		public User? Requestor { get; set; }

		public Department? Department { get; set; }

		public User? Agent { get; set; }

		public Category? Category { get; set; }

		public Status? Status { get; set; }

		public Priority? Priority { get; set; }

		public ICollection<Comment> Comments { get; set; } = new List<Comment>();

		public ICollection<TicketRating> Ratings { get; set; } = new List<TicketRating>();

		public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();
	}
}