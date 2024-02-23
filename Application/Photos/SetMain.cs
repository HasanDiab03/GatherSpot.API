using Application.Core;
using Application.Repositories;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Photos
{
	public record SetMainCommand(Guid Id) : IRequest<Result<Unit>>;
	public class SetMainHandler : IRequestHandler<SetMainCommand, Result<Unit>>
	{
		private readonly DataContext _context;
		private readonly IUserAccessor _userAccessor;

		public SetMainHandler(DataContext context, IUserAccessor userAccessor)
		{
			_context = context;
			_userAccessor = userAccessor;
		}
        public async Task<Result<Unit>> Handle(SetMainCommand request, CancellationToken cancellationToken)
        {
	        var user = await _context.Users
		        .Include(u => u.Photos)
		        .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
	        if (user is null)
		        return null;
			var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
			if(photo is null) return null;
			var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
			if(currentMain is not null) currentMain.IsMain = false;
			photo.IsMain = true;
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to set photo to main");
        }
	}
}
