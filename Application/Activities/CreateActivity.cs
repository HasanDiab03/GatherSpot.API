using Domain;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using FluentValidation;

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

		public CreateActivityHandler(DataContext context)
		{
			_context = context;
		}

		public async Task<Result<Unit>> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
		{
			_context.Activities.Add(request.Activity);
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result ? Result<Unit>.Success(Unit.Value) 
				: Result<Unit>.Failure("Failed To Create Activity");
		}
	}
}
