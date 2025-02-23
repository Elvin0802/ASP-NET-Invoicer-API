using Microsoft.AspNetCore.Identity;

namespace InvoicerAPI.Core.Entities;

public class User : IdentityUser<Guid>
{
	public string Name { get; set; }
	public string? Address { get; set; }
	public IList<Customer> Customers { get; set; }
	public IList<Invoice> Invoices { get; set; }
	public string? RefreshToken { get; set; }

	/// <summary>
	/// It sets with creating.
	/// </summary>
	public DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// It changes with every update.
	/// </summary>
	public DateTimeOffset UpdatedAt { get; set; }

	/// <summary>
	/// It changes on soft delete.
	/// </summary>
	public DateTimeOffset DeletedAt { get; set; }

}