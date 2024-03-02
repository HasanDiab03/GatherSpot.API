using Application.Core;
using Application.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
		private readonly IUserAccessor _userAccessor;

		public GetActivityHandler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
		{
			_context = context;
			_mapper = mapper;
			_userAccessor = userAccessor;
		}
		public async Task<Result<ActivityDto>> Handle(GetActivityQuery request, CancellationToken cancellationToken)
		{
			var activity = await _context.Activities
				.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
					new {currentUsername = _userAccessor.GetUsername()})
				.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
			return Result<ActivityDto>.Success(activity);
		}
	}
}
