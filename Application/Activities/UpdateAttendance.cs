using Application.Core;
using Application.Repositories;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
	public record UpdateAttendanceCommand(Guid Id) : IRequest<Result<Unit>>;
	public class UpdateAttendanceHandler : IRequestHandler<UpdateAttendanceCommand, Result<Unit>>
	{
		private readonly IUserAccessor _userAccessor;
		private readonly DataContext _context;
		private readonly ILogger<UpdateActivityHandler> _logger;

		public UpdateAttendanceHandler(IUserAccessor userAccessor, DataContext context, ILogger<UpdateActivityHandler> logger)
		{
			_userAccessor = userAccessor;
			_context = context;
			_logger = logger;
		}
        public async Task<Result<Unit>> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities
				.Include(a => a.Attendees)
				.ThenInclude(u => u.AppUser)
				.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
			// this would not cause the circular loop error, since we are not serializing the data.
			// it often occurs when we would return such thing as response, so when transforming to json, it would lead to error
			if (activity is null)
				return null;
			var user = await _context.Users
				.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
			if(user is null)
				return null;
			var hostUsername = activity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;
			var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);
			if (attendance is not null && hostUsername == user.UserName)
			{
				activity.IsCancelled = !activity.IsCancelled;
			} // user making request is host

			if (attendance is not null && hostUsername != user.UserName)
			{
				activity.Attendees.Remove(attendance);
			}

			if (attendance is null)
			{
				attendance = new ActivityAttendee()
				{
					AppUser = user,
					Activity = activity,
					IsHost = false
				};
				activity.Attendees.Add(attendance);
			}

			var result = await _context.SaveChangesAsync(cancellationToken) > 0;

			return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem Updating Attendance");
		}
	}
}
