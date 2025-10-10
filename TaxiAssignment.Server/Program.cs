using Serilog;

using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) =>
{
	loggerConfig.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
	.AddNewtonsoftJson();
builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<IGenerateDataService, GenerateDataService>();
builder.Services.AddSingleton<IGeoPointsService, GeoPointsService>();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("http://localhost:5173")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started and is now listening for requests.");

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();