using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class CategoriesController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}