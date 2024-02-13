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
	public record GetActivityQuery(Guid Id) : IRequest<Activity>;
	public class GetActivityHandler : IRequestHandler<GetActivityQuery, Activity>
	{
		private readonly DataContext _context;

		public GetActivityHandler(DataContext context)
		{
			_context = context;
		}
		public async Task<Activity> Handle(GetActivityQuery request, CancellationToken cancellationToken)
			=> await _context.Activities.FindAsync(request.Id);
	}
}
