using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_System.Controllers {
	public class DepartmentsController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}