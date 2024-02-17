using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	[AllowAnonymous]
	public class ActivitiesController : BaseApiController
	{

		[HttpGet]
		public async Task<ActionResult<List<Activity>>> GetActivities(CancellationToken ct)
		{
			var result = await Mediator.Send(new GetActivitiesQuery(), ct);
			return HandleResult(result);
		}																	// send an instance of the GetActivitiesQuery,
																		   // which will be handled by its own RequestHandler

		[HttpGet("{id}")]
		public async Task<IActionResult> GetActivity(Guid id)
		{
			var result = await Mediator.Send(new GetActivityQuery(id));
			return HandleResult(result);
		}	

		[HttpPost]
		public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
		{
			var result = await Mediator.Send(new CreateActivityCommand(activity));
			return HandleResult(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateActivity(Guid id, Activity activity)
		{
			activity.Id = id;
			var result = await Mediator.Send(new UpdateActivityCommand(activity));
			return HandleResult(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteActivity(Guid id)
		{
			var result = await Mediator.Send(new DeleteActivityCommand(id));
			return HandleResult(result);
		}
	}
}
