using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class ProfilesController : BaseApiController
	{
		[HttpGet("{username}")]
		public async Task<IActionResult> GetProfile(string username, CancellationToken cancellationToken)
			=> HandleResult(await Mediator.Send(new GetUserProfileQuery(username), cancellationToken));
	}
}
