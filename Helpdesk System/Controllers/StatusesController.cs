using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class StatusesController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}