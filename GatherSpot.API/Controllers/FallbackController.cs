using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	[AllowAnonymous]
	public class FallbackController : Controller
	{// since we are serving the react app from the .net kestrel, whenever routing happens in react
	 // it will trigger the API routing, so this controller is used to serve the index.html page
	 // that takes care of the react routing.
		public IActionResult Index()
		{
			return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
				"wwwroot", "index.html"), "text/HTML");
		}
	}
}
