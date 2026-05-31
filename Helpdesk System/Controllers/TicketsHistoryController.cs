using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class TicketsHistoryController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}