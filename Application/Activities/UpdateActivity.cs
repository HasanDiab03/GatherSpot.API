using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
	public record UpdateActivityCommand(Activity Activity) : IRequest;
	public class UpdateActivityHandler : IRequestHandler<UpdateActivityCommand>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public UpdateActivityHandler(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities.FindAsync(request.Activity.Id);
			if (activity is not null)
			{
				_mapper.Map(request.Activity, activity);
				await _context.SaveChangesAsync();
			}
		}
	}
}
