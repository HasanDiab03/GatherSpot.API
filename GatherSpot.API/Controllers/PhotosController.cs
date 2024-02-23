using Application.Photos;
using Application.Profiles;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	public class PhotosController : BaseApiController
	{
		[HttpPost]
		public async Task<IActionResult> Add([FromForm] AddPhotoCommand command, CancellationToken cancellationToken)
			=> HandleResult(await Mediator.Send(command, cancellationToken));

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
			=> HandleResult(await Mediator.Send(new DeletePhotoCommand(id), cancellationToken));

		[HttpPost("{id}/setMain")]
		public async Task<IActionResult> SetMain(Guid id, CancellationToken cancellationToken)
			=> HandleResult(await Mediator.Send(new SetMainCommand(id), cancellationToken));
	}
}
