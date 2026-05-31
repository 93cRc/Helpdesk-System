using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class TicketsController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}