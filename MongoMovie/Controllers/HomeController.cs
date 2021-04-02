using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MongoMovie.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Mongo Movie ";
			return View();
		}
		
		public ActionResult About()
		{
			ViewBag.Message = " About us";
			return View();
		}

		public ActionResult Reviews()
		{
			ViewBag.Message = "Welcome to Review our recipes";
			return View();
		}		
	}
}