using Application.Activities;
using Application.Core;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Reflection;
using Application.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using Infrastructure.Email;

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
				options.UseSqlite(config.GetConnectionString("SQLiteConnection"));
			});
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", policy =>
				{
					policy
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials() // this is for signalR specificly, since it requires Access-Control-Allow-Credentials header to be sent
						.WithOrigins("http://localhost:5173", "https://localhost:5173");
				});
			});

			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(GetActivitiesQuery))));
			services.AddAutoMapper(typeof(MappingProfiles).Assembly);
			services.AddFluentValidationAutoValidation();
			services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
			services.AddHttpContextAccessor();
			services.AddScoped<IUserAccessor, UserAccessor>();
			services.AddScoped<IPhotoAccessor, PhotoAccessor>();
			services.AddScoped<EmailSender>();
			services.AddSignalR();
			return services;
		}
	}
}
