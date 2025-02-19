using InvoicerAPI;
using InvoicerAPI.Application.Services;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

