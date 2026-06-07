using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.TicketRatings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helpdesk_System.Controllers {
	[Authorize(Roles = "Admin,Requestor")]
	public class TicketRatingsController : Controller {
		private const int ResolvedStatusId = 6;
		private const int ClosedStatusId = 7;

		private readonly ITicketRatingService _ticketRatingService;

		public TicketRatingsController(ITicketRatingService ticketRatingService) {

			_ticketRatingService = ticketRatingService;
		}

		[HttpGet]
		public async Task<IActionResult> Create(int ticketId) {
			var currentUserId = GetCurrentUserId();

			if (currentUserId == null) {
				return Unauthorized();
			}

			var ticket = await _ticketRatingService.GetTicketAsync(ticketId);

			if (ticket == null) {
				return NotFound();
			}

            if (!User.IsInRole("Admin") && ticket.RequestorId != currentUserId.Value)
            {
                return Forbid();
            }

            if (ticket.StatusId != ResolvedStatusId && ticket.StatusId != ClosedStatusId) {

				return RedirectToAction("Details", "Tickets", new { id = ticketId });
			}

			if (await _ticketRatingService.HasRatingAsync(ticketId)) {
				return BadRequest("To zgłoszenie zostało już ocenione.");
			}

			var model = new CreateTicketRatingViewModel {
				TicketId = ticket.Id,
				TicketTitle = ticket.Title
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateTicketRatingViewModel model) {

			var currentUserId = GetCurrentUserId();

			if (currentUserId == null) {
				return Unauthorized();
			}

			var ticket = await _ticketRatingService
				.GetTicketAsync(model.TicketId);

			if (ticket == null) {
				return NotFound();
			}

			if (ticket.RequestorId != currentUserId.Value) {
				return Forbid();
			}

			model.TicketTitle = ticket.Title;

			if (!ModelState.IsValid) {
				return View(model);
			}

			try {
                await _ticketRatingService.CreateAsync(
                    model.TicketId,
                    currentUserId.Value,
                    model.Rating,
                    model.Content,
                    User.IsInRole("Admin"));
            }
			catch (KeyNotFoundException) {
				return NotFound();
			}
			catch (UnauthorizedAccessException) {
				return Forbid();
			}
			catch (InvalidOperationException exception) {
				ModelState.AddModelError(string.Empty, exception.Message);
				return View(model);
			}
			catch (ArgumentOutOfRangeException exception) {
				ModelState.AddModelError(nameof(model.Rating), exception.Message);
				return View(model);
			}

			return RedirectToAction("Details", "Tickets", new { id = model.TicketId });
		}

		private int? GetCurrentUserId() {
			var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!int.TryParse(userIdClaim, out var userId)) {
				return null;
			}

			return userId;
		}
	}
}