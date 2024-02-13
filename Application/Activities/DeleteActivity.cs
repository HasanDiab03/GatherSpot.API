using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Activities
{
	public record DeleteActivityCommand(Guid Id) : IRequest;
	public class DeleteActivityHandler : IRequestHandler<DeleteActivityCommand>
	{
		private readonly DataContext _context;

		public DeleteActivityHandler(DataContext context)
		{
			_context = context;
		}
		public async Task Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities.FindAsync(request.Id);
			_context.Remove(activity);
			await _context.SaveChangesAsync();
		}
	}
}
