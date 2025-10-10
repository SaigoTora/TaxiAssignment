using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Services;

var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();