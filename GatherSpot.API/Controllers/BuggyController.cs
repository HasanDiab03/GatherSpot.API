using System;
using GatherSpot.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class BuggyController : BaseApiController
	{
		[HttpGet("not-found")]
		public ActionResult GetNotFound()
		{
			return NotFound();
		}

		[HttpGet("bad-request")]
		public ActionResult GetBadRequest()
		{
			return BadRequest("This is a bad request");
		}

		[HttpGet("server-error")]
		public IActionResult GetServerError()
		{
			throw new Exception("This is a server error");
		}

		[HttpGet("unauthorised")]
		public ActionResult GetUnauthorised()
		{
			return Unauthorized();
		}
	}
}