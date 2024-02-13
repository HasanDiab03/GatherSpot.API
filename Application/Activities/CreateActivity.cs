using Domain;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities
{
	public record CreateActivityCommand(Activity Activity) : IRequest;
	public class CreateActivityHandler : IRequestHandler<CreateActivityCommand>
	{
		private readonly DataContext _context;

		public CreateActivityHandler(DataContext context)
		{
			_context = context;
		}

		public async Task Handle(CreateActivityCommand request, CancellationToken cancellationToken)
		{
			_context.Activities.Add(request.Activity);
			await _context.SaveChangesAsync();
		}
	}
}
