using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Application.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public record UpdateProfileCommand(string DisplayName, string Bio = "") : IRequest<Result<Unit>>;

	public class UpdateProfileValidator: AbstractValidator<UpdateProfileCommand>
	{
		public UpdateProfileValidator()
		{
			RuleFor(upc => upc.DisplayName).NotEmpty();
		}
	}
	public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result<Unit>>
	{
		private readonly DataContext _context;
		private readonly IUserAccessor _userAccessor;

		public UpdateProfileHandler(DataContext context, IUserAccessor userAccessor)
		{
			_context = context;
			_userAccessor = userAccessor;
		}
		public async Task<Result<Unit>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
			user.DisplayName = request.DisplayName ?? user.DisplayName;
			user.Bio = request.Bio ?? user.Bio;
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed To Update Profile");
		}
	}
}
