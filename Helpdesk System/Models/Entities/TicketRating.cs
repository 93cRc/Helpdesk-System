namespace Helpdesk_System.Models.Entities {
	public class TicketRating {
		public int Id { get; set; }

		public int TicketId { get; set; }

		public int UserId { get; set; }

		public byte Rating { get; set; }

		public string? Content { get; set; }

		public DateTime CreatedAt { get; set; }

		public Ticket? Ticket { get; set; }

		public User? User { get; set; }
	}
}