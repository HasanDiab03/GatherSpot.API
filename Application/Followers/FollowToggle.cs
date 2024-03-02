using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Application.Repositories;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
	public record FollowToggleCommand(string TargetUsername) : IRequest<Result<Unit>>;

	public class FollowToggleHandler : IRequestHandler<FollowToggleCommand, Result<Unit>>
	{
		private readonly DataContext _context;
		private readonly IUserAccessor _userAccessor;

		public FollowToggleHandler(DataContext context, IUserAccessor userAccessor)
		{
			_context = context;
			_userAccessor = userAccessor;
		}
		public async Task<Result<Unit>> Handle(FollowToggleCommand request, CancellationToken cancellationToken)
		{
			var observer = await _context.Users
				.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
			var target = await _context.Users
				.FirstOrDefaultAsync(x => x.UserName == request.TargetUsername, cancellationToken);
			if (target is null)
				return null;
			var following = await _context.UsersFollowings
				.FindAsync(observer.Id, target.Id, cancellationToken);
			if (following is null)
			{
				following = new UserFollowing()
				{
					Observer = observer,
					Target = target
				};
				_context.UsersFollowings.Add(following);
			}
			else
			{
				_context.UsersFollowings.Remove(following);
			}

			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to Follow/UnFollow");
		}
	}
}
