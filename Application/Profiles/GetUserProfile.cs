using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public record GetUserProfileQuery(string Username) : IRequest<Result<Profile>>;

	public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, Result<Profile>>
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public GetUserProfileHandler(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<Result<Profile>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
		{
			var user = await _context.Users
				.ProjectTo<Profile>(_mapper.ConfigurationProvider)
				.FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
			return Result<Profile>.Success(user);
		}
	}

}
