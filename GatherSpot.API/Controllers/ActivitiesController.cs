using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class ActivitiesController : BaseApiController
	{
		
		[HttpGet]
		public async Task<ActionResult<List<Activity>>> GetActivities(CancellationToken ct)
			=> await Mediator.Send(new GetActivitiesQuery(), ct); // send an instance of the GetActivitiesQuery,
																		// which will be handled by its own RequestHandler

		[HttpGet("{id}")]
		public async Task<ActionResult<Activity>> GetActivity(Guid id)
			=> await Mediator.Send(new GetActivityQuery(id));

		[HttpPost]
		public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
		{
			await Mediator.Send(new CreateActivityCommand(activity));
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateActivity(Guid id, Activity activity)
		{
			activity.Id = id;
			await Mediator.Send(new UpdateActivityCommand(activity));
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteActivity(Guid id)
		{
			await Mediator.Send(new DeleteActivityCommand(id));
			return Ok();
		}
	}
}
