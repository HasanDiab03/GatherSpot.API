using System.Text;
using Domain;
using GatherSpot.API.Services;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace GatherSpot.API.Extensions
{
	public static class IdentityServiceExtensions
	{
		public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddIdentityCore<AppUser>(options =>
			{
				options.Password.RequireNonAlphanumeric = false; // we can add options to a couple of attributes here such as the password
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<DataContext>(); // this is used to allow to connect and query the identity tables in our db
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration["TokenKey"]));
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = key,
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ClockSkew = TimeSpan.Zero // this is because be default, there will always be a 5 minute window that the token is still valid in, so we remove it
					};
					options.Events = new JwtBearerEvents()
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];
							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
							{
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						}
					};
				}); // this is to verify the jwt we receive,
					// it will verify that the token is signed with the same key that we used to sign it before.
			services.AddScoped<TokenService>();
			services.AddAuthorization(options =>
			{
				options.AddPolicy("IsActivityHost", policy =>
				{
					policy.Requirements.Add(new IsHostRequirement());
				});
			}); // this is to configure the authorization to add a new policy that we can use on our endpoints
			services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
			return services;
		}
	}
}
