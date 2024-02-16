using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
	public record GetActivityQuery(Guid Id) : IRequest<Result<Activity>>;
	public class GetActivityHandler : IRequestHandler<GetActivityQuery, Result<Activity>>
	{
		private readonly DataContext _context;

		public GetActivityHandler(DataContext context)
		{
			_context = context;
		}

		public async Task<Result<Activity>> Handle(GetActivityQuery request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities.FindAsync(request.Id);
			return Result<Activity>.Success(activity);
		}
	}
}
