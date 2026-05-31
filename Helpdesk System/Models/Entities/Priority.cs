namespace Helpdesk_System.Models.Entities {
	public class Priority {
		public int Id { get; set; }

		public string Code { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public int SortOrder { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
	}
}