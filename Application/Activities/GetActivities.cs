using Application.Core;
using Application.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public record GetActivitiesQuery(ActivityParams Params): IRequest<Result<PagedList<ActivityDto>>>;
	public class GetActivitiesHandler : IRequestHandler<GetActivitiesQuery, Result<PagedList<ActivityDto>>>
	{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly IUserAccessor _userAccessor;

			public GetActivitiesHandler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
			{
				_context = context;
				_mapper = mapper;
				_userAccessor = userAccessor;
			}

			public async Task<Result<PagedList<ActivityDto>>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
			{
				var query = _context.Activities
					.Where(x => x.Date >= request.Params.StartDate)
					.OrderBy(a => a.Date)
					.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider,
						new { currentUsername = _userAccessor.GetUsername() })
					.AsQueryable();

				if (request.Params.IsGoing && !request.Params.IsHost)
				{
					query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
				}

				if (request.Params.IsHost && !request.Params.IsGoing)
				{
					query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
				}

				//	ProjectTo is equivilent to Include(), it just does it in a cleaner way.
				return Result<PagedList<ActivityDto>>.Success(
					await PagedList<ActivityDto>.CreateAsync(
						query, request.Params.PageNumber, request.Params.PageSize));
			}
			// The Cancellation Token is used for cancelling the request whenever a client no longer wants to keep waiting until it finishes,
			// and no longer needs this request. By Default, even if a client actually closes the request,
			// by not using a cancellation token, the request will still go on in the background
	}
}
