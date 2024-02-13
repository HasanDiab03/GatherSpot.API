using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
	public record GetActivitiesQuery: IRequest<List<Activity>>;
	public class GetActivitiesHandler : IRequestHandler<GetActivitiesQuery, List<Activity>>
	{
			private readonly DataContext _context;

			public GetActivitiesHandler(DataContext context)
			{
				_context = context;
			}

			public async Task<List<Activity>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
			=> await _context.Activities.ToListAsync(cancellationToken);
			// The Cancellation Token is used for cancelling the request whenever a client no longer wants to keep waiting until it finishes,
			// and no longer needs this request. By Default, even if a client actually closes the request,
			// by not using a cancellation token, the request will still go on in the background
	}
}
