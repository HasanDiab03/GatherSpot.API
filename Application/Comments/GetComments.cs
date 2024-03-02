using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments
{
	public record GetCommentsQuery(Guid ActivityId) : IRequest<Result<List<CommentDto>>>;
	public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, Result<List<CommentDto>>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public GetCommentsHandler(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
        public async Task<Result<List<CommentDto>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
		{
			var comments = await _context.Comments
				.Where(x => x.Activity.Id == request.ActivityId)
				.OrderByDescending(x => x.CreatedAt)
				.ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
				.ToListAsync(cancellationToken);
			return Result<List<CommentDto>>.Success(comments);
		}
	}
}
