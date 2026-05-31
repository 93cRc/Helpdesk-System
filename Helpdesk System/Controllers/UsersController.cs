using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class UsersController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}