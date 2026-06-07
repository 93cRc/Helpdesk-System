using Helpdesk_System.Models.Entities;

namespace Helpdesk_System.Services.Interfaces {
	public interface ITicketRatingService {
		Task<Ticket?> GetTicketAsync(int ticketId);

		Task<bool> HasRatingAsync(int ticketId);

		Task CreateAsync(int ticketId, int userId, byte rating, string? content);
	}
}