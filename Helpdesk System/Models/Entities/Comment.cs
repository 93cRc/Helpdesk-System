namespace Helpdesk_System.Models.Entities {
	public class Comment {
		public int Id { get; set; }

		public int TicketId { get; set; }

		public int UserId { get; set; }

		public string Content { get; set; } = string.Empty;

		public bool IsInternal { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? DeletedAt { get; set; }

		public Ticket? Ticket { get; set; }

		public User? User { get; set; }
	}
}