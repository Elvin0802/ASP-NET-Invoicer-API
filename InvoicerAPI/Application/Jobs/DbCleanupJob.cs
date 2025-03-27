using InvoicerAPI.Core.Enums;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace InvoicerAPI.Application.Jobs;

public class DbCleanupJob : IJob
{
	private readonly IServiceProvider _provider;

	public DbCleanupJob(IServiceProvider provider)
	{
		_provider = provider;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		using var scope = _provider.CreateScope();

		var dbContext = scope.ServiceProvider.GetRequiredService<InvoicerDbContext>();

		var list = await dbContext.Invoices
			.Where(invoice => invoice.CreatedAt < DateTimeOffset.UtcNow.AddDays(-14) &&
							 (invoice.Status == InvoiceStatus.Created ||
							  invoice.Status == InvoiceStatus.Cancelled ||
							  invoice.Status == InvoiceStatus.Rejected))
			.Include(invoice => invoice.Rows)
			.ToListAsync();

		if (list.Count > 0)
		{
			dbContext.Invoices.RemoveRange(list);
			await dbContext.SaveChangesAsync();
		}
	}
}
