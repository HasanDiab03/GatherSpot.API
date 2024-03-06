using Domain;
using MediatR;
using Persistence;
using Application.Core;
using Application.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
	public record CreateActivityCommand(Activity Activity) : IRequest<Result<Unit>>;

	public class CreateActivityValidator : AbstractValidator<CreateActivityCommand>
	{
		public CreateActivityValidator()
		{
			RuleFor(cac => cac.Activity).SetValidator(new ActivityValidator());
		}
	}
	public class CreateActivityHandler : IRequestHandler<CreateActivityCommand, Result<Unit>>
	{
		private readonly DataContext _context;
		private readonly IUserAccessor _userAccessor;

		public CreateActivityHandler(DataContext context, IUserAccessor userAccessor)
		{
			_context = context;
			_userAccessor = userAccessor;
		}

		public async Task<Result<Unit>> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername(), cancellationToken);
			var attendee = new ActivityAttendee()
			{
				AppUser = user,
				Activity = request.Activity,
				IsHost = true
			};
			request.Activity.Attendees.Add(attendee);
			_context.Activities.Add(request.Activity);
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) 
				: Result<Unit>.Failure("Failed To Create Activity");
		}
	}
}
