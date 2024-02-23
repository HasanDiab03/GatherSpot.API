using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
	public class IsHostRequirement : IAuthorizationRequirement
	{
	}
	// IAuthorizationRequirement is an interface that represents a requirement that must be met for authorization to succeed
	// what we are doing here is adding more requirements to our authorization
	public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
	{
		private readonly DataContext _dbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_httpContextAccessor = httpContextAccessor;
		}
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
		{
			var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if(userId == null)
				return Task.CompletedTask;
			var activityId = Guid.Parse(_httpContextAccessor
				.HttpContext.Request
				.RouteValues.SingleOrDefault(x => x.Key == "id").Value.ToString());

			var attendee = _dbContext.ActivityAttendees
				.AsNoTracking() // this is used so that this variable doesn't stay in memory and be kept track of, so that it doesn't mess things for entity framework
				.SingleOrDefaultAsync(x=> x.AppUserId == userId && x.ActivityId ==  activityId).Result;
			
			if (attendee is null)
				return Task.CompletedTask;
			if (attendee.IsHost)
			{
				context.Succeed(requirement);
			}
			return Task.CompletedTask;
		}
	}
}
