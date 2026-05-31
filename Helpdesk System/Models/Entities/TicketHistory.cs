namespace Helpdesk_System.Models.Entities {
	public class TicketHistory {
		public int Id { get; set; }

		public int TicketId { get; set; }

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

		public int? ChangedBy { get; set; }

		public DateTime HistoryCreatedAt { get; set; }

		public Ticket? Ticket { get; set; }

		public User? Requestor { get; set; }

		public Department? Department { get; set; }

		public User? Agent { get; set; }

		public Category? Category { get; set; }

		public Status? Status { get; set; }

		public Priority? Priority { get; set; }

		public User? ChangedByUser { get; set; }
	}
}