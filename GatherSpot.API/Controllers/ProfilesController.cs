using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class ProfilesController : BaseApiController
	{
		[HttpGet("{username}")]
		public async Task<IActionResult> GetProfile(string username, CancellationToken cancellationToken)
			=> HandleResult(await Mediator.Send(new GetUserProfileQuery(username), cancellationToken));

		[HttpPut]
		public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command, CancellationToken cancellationToken)
			=> HandleResult(await Mediator.Send(command, cancellationToken));

		[HttpGet("events/{username}")]
		public async Task<IActionResult> GetUserEvents(string username, [FromQuery] string predicate, CancellationToken ct)
			=> HandleResult(await Mediator.Send(new GetUserEventsQuery(username, predicate), ct));
	}
}
