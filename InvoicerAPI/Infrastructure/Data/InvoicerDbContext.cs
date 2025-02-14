using InvoicerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoicerAPI.Infrastructure.Data;

public class InvoicerDbContext : DbContext
{
	public InvoicerDbContext(DbContextOptions options) : base(options)
	{ }

	public DbSet<Invoice> Invoices { get; set; }
	public DbSet<InvoiceRow> InvoiceRows { get; set; }
	public DbSet<User> Users { get; set; }
}
