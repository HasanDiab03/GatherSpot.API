using Application.Core;
using GatherSpot.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatherSpot.API.Controllers
{
	[ApiController] // for error handling, when having a validation annotation for an attribute in our domain,
                 // the controller automatically sends a 400 error code when validation fails,
                 // it adds an error to the ModelState, and checks if its valid and sends bad request if invalid 
	[Route("api/[controller]")]
	public class BaseApiController : ControllerBase
	{
		private IMediator _mediator;
		protected IMediator Mediator => _mediator ??=HttpContext.RequestServices.GetService<IMediator>();
		// if there is a mediator, it will just return it, otherwise if it's null it will get a new one from the container 
		protected ActionResult HandleResult<T>(Result<T> result)
		{
			if (result is null) return NotFound();
			if (result.IsSuccess && result.Value is not null)
			{
				return Ok(result.Value);
			}
			if (result.IsSuccess && result.Value is null)
			{
				return NotFound();
			}
			return BadRequest(result.Error);
		}

		protected ActionResult HandlePagedResult<T>(Result<PagedList<T>> result)
		{
			if (result is null) return NotFound();
			if (result.IsSuccess && result.Value is not null)
			{
				Response.AddPaginationHeader(
					result.Value.CurrentPage, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages);
				return Ok(result.Value);
			}
			if (result.IsSuccess && result.Value is null)
			{
				return NotFound();
			}
			return BadRequest(result.Error);
		}
	}
}
