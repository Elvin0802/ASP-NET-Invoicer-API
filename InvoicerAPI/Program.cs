using InvoicerAPI;
using InvoicerAPI.Application.Services.Invoice;
using InvoicerAPI.Application.Services.User;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IUserService, UserService>();



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

app.UseAuthorization();

app.MapControllers();

app.Run();



/*

Invoicer API:
1. Entities:
• User:
- Id: Guid
- Name: String
- Address: string?
- Email: string
- Password: string
- PhoneNumber: string?
- CreatedAt: DateTimeOffset
- UpdatedAt: DateTimeOffset // İstənilən informasiya dəyişikliyi zamanı yenilənir
- DeletedAt: DateTimeOffset // Soft delete üçün

• Invoice:
- Id: Guid
- CustomerId: Guid
- StartDate: DateTimeOffset // İşin yerinə yetirilmə müddətinin başlanğıcı
- EndDate: DateTimeOffset // İşin yerinə yetirilmə müddətinin sonu
- Rows: InvoiceRow[]
- TotalSum: decimal // bütün Rows-ların cəmi
- Comment: string?
- Status: InvoiceStatus(Created, Sent, Received, Paid, Cancelled,
Rejected)
- CreatedAt: DateTimeOffset
- UpdatedAt: DateTimeOffset // İstənilən informasiya dəyişikliyi zamanı
yenilənir
- DeletedAt: DateTimeOffset // Soft delete üçün

• InvoiceRow:
- Id: Guid
- InvoiceId: Guid
- Service: string // İşin adı
- Quantity: decimal // görülən işin miqdarı(saat, gün, kq, dənə və s)
- Amount: decimal // işin qiyməti
- Sum: decimal // ümumi məbləğ (Quantity və Amount əsasında
alınır)

    API aşağıdakı əməliyyatları dəstəkləməlidir:
    • Invoces:

    1. Create invoice

    2. Edit invoice - yalnız göndərilməmiş invoice-lar edit oluna bilər

    3. Change invoice status

    4. Delete invoice (hard delete) - yalnız göndərilməmiş invoice-ləri silmək olar.

    5. Archive invoice (soft delete)

    6. Get invoices list (pagination, filters, sorting)

    7. Get invoice by Id

*/
