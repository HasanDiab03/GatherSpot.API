using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BaseApiController : ControllerBase
	{
		private IMediator _mediator;
		protected IMediator Mediator => _mediator ??=HttpContext.RequestServices.GetService<IMediator>();
		// if there is a mediator, it will just return it, otherwise if it's null it will get a new one from the container 
	}
}
