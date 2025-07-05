using FluentValidation;
using FluentValidation.AspNetCore;
using InvoicerAPI;
using InvoicerAPI.Application.Services;
using InvoicerAPI.Application.Validation.Validators.Customers;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddSwagger();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddDbContext<InvoicerDbContext>(
	options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
	});

builder.Services.AddHealthChecks()
.AddCheck("self", () => HealthCheckResult.Healthy()); // Liveness.

builder.Services.AddCors(options => options.AddPolicy("CORSPolicy", builder =>
{
	builder.AllowAnyMethod()
			   .AllowAnyHeader()
			   .WithOrigins("http://localhost:3000", "https://lvn-invoicer-app.vercel.app")
			   .AllowCredentials();
}));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(BaseCustomerValidator).Assembly);

builder.Services.AddQuartzJobs();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// LIVENESS
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
	Predicate = check => check.Name == "self"
});

app.MapGet("/health/", () => "The API is working successfully.");

app.Run();

