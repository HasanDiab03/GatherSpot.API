using Application.Core;
using Application.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Profile = Application.Profiles.Profile;

namespace Application.Followers
{
	public record GetFollowersQuery(string Username, string Predicate) : IRequest<Result<List<Profile>>>;

	public class GetFollowersHandler : IRequestHandler<GetFollowersQuery, Result<List<Profile>>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly IUserAccessor _userAccessor;

		public GetFollowersHandler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
		{
			_context = context;
			_mapper = mapper;
			_userAccessor = userAccessor;
		}
		public async Task<Result<List<Profile>>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
		{
			var profiles = new List<Profile>();
			switch (request.Predicate)
			{
				case "followers":
					profiles = await _context.UsersFollowings
						.Where(x => x.Target.UserName == request.Username)
						.Select(u => u.Observer) // to only get the observer, since the target would be requester
						.ProjectTo<Profile>(_mapper.ConfigurationProvider,
							new {currentUsername = _userAccessor.GetUsername()}) // we can pass a param to the Mapping Profiles Configuration like this
						.ToListAsync(cancellationToken);
					break;
				case "following":
					profiles = await _context.UsersFollowings
						.Where(x => x.Observer.UserName == request.Username)
						.Select(u => u.Target) // to only get the target, since the observer would be requester
						.ProjectTo<Profile>(_mapper.ConfigurationProvider,
							new { currentUsername = _userAccessor.GetUsername() })
						.ToListAsync(cancellationToken);
					break;
			}
			return Result<List<Profile>>.Success(profiles);
		}
	}

}
