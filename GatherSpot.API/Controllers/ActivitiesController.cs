using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class ActivitiesController : BaseApiController
	{

		[HttpGet]
		public async Task<ActionResult<List<Activity>>> GetActivities([FromQuery] ActivityParams pp,CancellationToken ct)
		{
			var result = await Mediator.Send(new GetActivitiesQuery(pp), ct);
			return HandlePagedResult(result);
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
		[Authorize(Policy = "IsActivityHost")] // this is an authorization policy, used so that only a user that is a host can update it
		public async Task<IActionResult> UpdateActivity(Guid id, Activity activity)
		{
			activity.Id = id;
			var result = await Mediator.Send(new UpdateActivityCommand(activity));
			return HandleResult(result);
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = "IsActivityHost")]
		public async Task<IActionResult> DeleteActivity(Guid id)
		{
			var result = await Mediator.Send(new DeleteActivityCommand(id));
			return HandleResult(result);
		}

		[HttpPost("{id}/attend")]
		public async Task<IActionResult> Attend(Guid id)
		{
			var result = await Mediator.Send(new UpdateAttendanceCommand(id));
			return HandleResult(result);
		}
	}
}
