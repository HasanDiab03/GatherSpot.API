using Application.Core;
using Application.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public record GetActivitiesQuery: IRequest<Result<List<ActivityDto>>>;
	public class GetActivitiesHandler : IRequestHandler<GetActivitiesQuery, Result<List<ActivityDto>>>
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

			public async Task<Result<List<ActivityDto>>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
			{
				var activities = await _context.Activities
					.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
						new {currentUsername = _userAccessor.GetUsername()})
					.ToListAsync(cancellationToken);
				//	ProjectTo is equivilent to Include(), it just does it in a cleaner way.
				return Result<List<ActivityDto>>.Success(activities);
			}
			// The Cancellation Token is used for cancelling the request whenever a client no longer wants to keep waiting until it finishes,
			// and no longer needs this request. By Default, even if a client actually closes the request,
			// by not using a cancellation token, the request will still go on in the background
	}
}
