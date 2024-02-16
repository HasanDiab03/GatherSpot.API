using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities
{
	public record DeleteActivityCommand(Guid Id) : IRequest<Result<Unit>>;
	public class DeleteActivityHandler : IRequestHandler<DeleteActivityCommand, Result<Unit>>
	{
		private readonly DataContext _context;

		public DeleteActivityHandler(DataContext context)
		{
			_context = context;
		}
		public async Task<Result<Unit>> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities.FindAsync(request.Id);
			if (activity is null)
				return null;
			_context.Remove(activity);
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed To Delete Activity");

		}
	}
}
