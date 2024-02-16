using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
	public record UpdateActivityCommand(Activity Activity) : IRequest<Result<Unit>>;

	public class UpdateActivityValidator : AbstractValidator<UpdateActivityCommand>
	{
		public UpdateActivityValidator()
		{
			RuleFor(uac => uac.Activity).SetValidator(new ActivityValidator());
		}
	}
	public class UpdateActivityHandler : IRequestHandler<UpdateActivityCommand, Result<Unit>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public UpdateActivityHandler(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<Result<Unit>> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities.FindAsync(request.Activity.Id);
			if (activity is null)
				return null;
			_mapper.Map(request.Activity, activity);
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed To Update Activity");
		}
	}
}
