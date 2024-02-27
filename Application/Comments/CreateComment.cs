using Application.Core;
using Application.Repositories;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
	public record CreateCommentCommand(string Body, Guid ActivityId) : IRequest<Result<CommentDto>>;

	public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
	{
		public CreateCommentValidator()
		{
			RuleFor(ccc => ccc.Body).NotEmpty();
		}
	}

	public class CreateCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly IUserAccessor _userAccessor;

		public CreateCommandHandler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
		{
			_context = context;
			_mapper = mapper;
			_userAccessor = userAccessor;
		}
		public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities.FindAsync(request.ActivityId, cancellationToken);
			if (activity is null)
				return null;
			var user = await _context.Users
				.Include(u => u.Photos)
				.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
			var comment = new Comment()
			{
				Body = request.Body,
				Author = user,
				Activity = activity
			};
			activity.Comments.Add(comment);
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;
			return result
				? Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment))
				: Result<CommentDto>.Failure("Failed to Add Comment");
		}
	}

}
