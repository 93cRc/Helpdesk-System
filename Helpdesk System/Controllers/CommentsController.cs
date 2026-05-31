using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class CommentsController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}