using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public record GetActivitiesQuery: IRequest<Result<List<Activity>>>;
	public class GetActivitiesHandler : IRequestHandler<GetActivitiesQuery, Result<List<Activity>>>
	{
			private readonly DataContext _context;

			public GetActivitiesHandler(DataContext context)
			{
				_context = context;
			}

			public async Task<Result<List<Activity>>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
			{
				var activities = await _context.Activities.ToListAsync(cancellationToken);
				return Result<List<Activity>>.Success(activities);
			}
			// The Cancellation Token is used for cancelling the request whenever a client no longer wants to keep waiting until it finishes,
			// and no longer needs this request. By Default, even if a client actually closes the request,
			// by not using a cancellation token, the request will still go on in the background
	}
}
