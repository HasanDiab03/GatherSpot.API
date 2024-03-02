using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class FollowController : BaseApiController
	{
		[HttpPost("{username}")]
		public async Task<IActionResult> Follow(string username, CancellationToken ct)
			=> HandleResult(await Mediator.Send(new FollowToggleCommand(username), ct));

		[HttpGet("{username}")]
		public async Task<IActionResult> GetFollowings(string username, string predicate, CancellationToken ct)
			=> HandleResult(await Mediator.Send(new GetFollowersQuery(username, predicate), ct));
	}
}
