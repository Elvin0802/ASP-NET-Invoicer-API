using InvoicerAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InvoicerAPI.Infrastructure.Data;

public class InvoicerDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
	public InvoicerDbContext(DbContextOptions<InvoicerDbContext> options) : base(options)
	{ }

	public DbSet<User> Users { get; set; }
	public DbSet<Invoice> Invoices { get; set; }
	public DbSet<InvoiceRow> InvoiceRows { get; set; }
	public DbSet<Customer> Customers { get; set; }
}
