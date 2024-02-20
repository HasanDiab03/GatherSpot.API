using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public record GetActivityQuery(Guid Id) : IRequest<Result<ActivityDto>>;
	public class GetActivityHandler : IRequestHandler<GetActivityQuery, Result<ActivityDto>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public GetActivityHandler(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<Result<ActivityDto>> Handle(GetActivityQuery request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities
				.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
				.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
			return Result<ActivityDto>.Success(activity);
		}
	}
}
