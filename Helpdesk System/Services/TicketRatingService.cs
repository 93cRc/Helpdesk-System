using Helpdesk_System.Data;
using Helpdesk_System.Models.Entities;
using Helpdesk_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_System.Services {
	public class TicketRatingService : ITicketRatingService {
		private const int ResolvedStatusId = 6;
		private const int ClosedStatusId = 7;

		private readonly HelpdeskSystemDbContext _context;

		public TicketRatingService(HelpdeskSystemDbContext context) {
			_context = context;
		}

		public async Task<Ticket?> GetTicketAsync(int ticketId) {
			return await _context.Tickets
				.AsNoTracking()
				.FirstOrDefaultAsync(t => t.Id == ticketId);
		}

		public async Task<bool> HasRatingAsync(int ticketId) {
			return await _context.TicketRatings
				.AnyAsync(r => r.TicketId == ticketId);
		}

		public async Task CreateAsync(int ticketId, int userId, byte rating, string? content) {
			if (rating < 1 || rating > 5) {
				throw new ArgumentOutOfRangeException(nameof(rating), "Ocena musi mieścić się w zakresie od 1 do 5.");
			}

			var ticket = await _context.Tickets
				.FirstOrDefaultAsync(t => t.Id == ticketId);

			if (ticket == null) {
				throw new KeyNotFoundException("Nie znaleziono zgłoszenia.");
			}

			if (ticket.RequestorId != userId) {
				throw new UnauthorizedAccessException("Tylko autor zgłoszenia może je ocenić.");
			}

			if (ticket.StatusId != ResolvedStatusId && ticket.StatusId != ClosedStatusId) {
				throw new InvalidOperationException("Można ocenić wyłącznie rozwiązane lub zamknięte zgłoszenie.");
			}

			var ratingAlreadyExists = await _context.TicketRatings
				.AnyAsync(r => r.TicketId == ticketId);

			if (ratingAlreadyExists) {
				throw new InvalidOperationException("To zgłoszenie zostało już ocenione.");
			}

			var ticketRating = new TicketRating {
				TicketId = ticketId,
				UserId = userId,
				Rating = rating,
				Content = string.IsNullOrWhiteSpace(content)
					? null
					: content.Trim(),
				CreatedAt = DateTime.Now
			};

			_context.TicketRatings.Add(ticketRating);
			await _context.SaveChangesAsync();
		}
	}
}