using Application.Core;
using System.Net;
using System.Text.Json;

namespace GatherSpot.API.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception e)
			{
				_logger.LogInformation("Back To Exception Middleware catch block");
				_logger.LogError(e, e.Message);
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				var response = _env.IsDevelopment()
					? new AppException(context.Response.StatusCode, e.Message, e.StackTrace?.ToString())
					: new AppException(context.Response.StatusCode, "Internal Server Error");
				var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
				var json = JsonSerializer.Serialize(response, options);
				await context.Response.WriteAsync(json);
			}
		}
	}
}
