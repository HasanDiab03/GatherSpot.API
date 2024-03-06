using Application.Core;
using Application.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public record GetUserEventsQuery(string Username, string Predicate) : IRequest<Result<List<UserActivityDto>>>;

	public class GetUserEventsHandler : IRequestHandler<GetUserEventsQuery, Result<List<UserActivityDto>>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public GetUserEventsHandler(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<Result<List<UserActivityDto>>> Handle(GetUserEventsQuery request, CancellationToken cancellationToken)
		{
			var query = _context.Activities
				.Where(x => x.Attendees.Any(a => a.AppUser.UserName == request.Username))
				.OrderBy(x => x.Date)
				.ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
				.AsQueryable();

			query = request.Predicate switch
			{
				"past" => query.Where(x => x.Date < DateTime.UtcNow),
				"host" => query.Where(x => x.HostUsername == request.Username),
				_ => query.Where(x => x.Date >= DateTime.UtcNow),
			};
			var result = await query.ToListAsync(cancellationToken);
			return Result<List<UserActivityDto>>.Success(result);
		}
	}
}
