using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope(); // create a scope to access a service,
                                              // something like what happens when a http request comes for example,
                                              // where an instance of the DataContext is retrieved from the DI container,
                                              // and then disposed of when done
var services = scope.ServiceProvider;

try
{
	var context = services.GetRequiredService<DataContext>(); // basically what happens when the http request occurs,
                                                           // getting an instance of the DataContext Service from the DI Container
	await context.Database.MigrateAsync(); // same as Update-Database (add migrations, and create db if not exist)
	await Seed.SeedData(context);
}
catch (Exception e)
{
	var logger = services.GetRequiredService<ILogger<Program>>();
	logger.LogError(e, "An error occured during migration");
}
app.Run();

