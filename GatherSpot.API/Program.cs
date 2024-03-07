using Domain;
using GatherSpot.API.Extensions;
using GatherSpot.API.Middleware;
using GatherSpot.API.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
	var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
	options.Filters.Add(new AuthorizeFilter(policy)); // this is used to add authentication to every single controller endpoint
});
builder.Services.AddApplicationServices(builder.Configuration); 
// this is an extension method, which is a method that extends the Services Collection here
// used to seperate the process of adding services to a different file

builder.Services.AddIdentityService(builder.Configuration); // extension method 

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

app.UseXContentTypeOptions(); // adds X-Content-Type-Options header to prevent Mime-sniffing of the content type, keep only the specified content type as the allowed one
app.UseReferrerPolicy(options => options.NoReferrer()); // to control how much info the browser includes when navigating away from the app 
app.UseXXssProtection(options => options.EnabledWithBlockMode()); // add the cross site scripting protection header
app.UseXfo(options => options.Deny()); // prevents the app from being used in an Iframe to protect against click-jacking attacks
app.UseCsp(options => options
	.BlockAllMixedContent()
	.StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com"))
	.FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
	.FormActions(s => s.Self())
	.FrameAncestors(s => s.Self())
	.ImageSources(s => s.Self().CustomSources("blob:").CustomSources("https://firebasestorage.googleapis.com"))
	.ScriptSources(s => s.Self())); // this is the main defense against cross site scripting attacks, used to whitelist some sources of content 


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	app.Use(async (context, next) =>
	{
		context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
		// header that enforces the use of https
		await next.Invoke();
	});
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();// this looks inside the wwwroot folder, and fishes out and serve any default named static files (index.html, index.css, index.js, etc...)
app.UseStaticFiles();

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");

using var scope = app.Services.CreateScope(); // create a scope to access a service,
                                              // something like what happens when a http request comes for example,
                                              // where an instance of the DataContext is retrieved from the DI container,
                                              // and then disposed of when done
var services = scope.ServiceProvider;

try
{
	var context = services.GetRequiredService<DataContext>(); // basically what happens when the http request occurs,
                                                           // getting an instance of the DataContext Service from the DI Container
                                                           
	var userManager = services.GetRequiredService<UserManager<AppUser>>();
	await context.Database.MigrateAsync(); // same as Update-Database (add migrations, and create db if not exist)
	await Seed.SeedData(context, userManager);
}
catch (Exception e)
{
	var logger = services.GetRequiredService<ILogger<Program>>();
	logger.LogError(e, "An error occurred during migration");
}
app.Run();


