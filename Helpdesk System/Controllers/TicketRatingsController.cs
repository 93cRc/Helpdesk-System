using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class TicketRatingsController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}