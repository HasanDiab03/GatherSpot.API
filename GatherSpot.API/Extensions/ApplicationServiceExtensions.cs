using Application.Activities;
using Application.Core;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace GatherSpot.API.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services,
			IConfiguration config) // extension method that extends an IServiceCollection, and takes whatever it is extending as parameter(this ...)
		{
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlite(config.GetConnectionString("DefaultConnection"));
			});
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", policy =>
				{
					policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:5173");
				});
			});

			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(GetActivitiesQuery))));
			services.AddAutoMapper(typeof(MappingProfiles).Assembly);
			services.AddFluentValidationAutoValidation();
			services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
			return services;
		}
	}
}
