using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace GatherSpot.API.Controllers
{
	public class ActivitiesController : BaseApiController
	{
		private readonly DataContext _context;

		public ActivitiesController(DataContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<List<Activity>>> GetActivities()
			=> await _context.Activities.ToListAsync();

		[HttpGet("{id}")]
		public async Task<ActionResult<Activity>> GetActivity(Guid id)
			=> await _context.Activities.FindAsync(id);
	}
}
