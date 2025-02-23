namespace InvoicerAPI.Core.Entities;

public class Customer
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public Guid UserId { get; set; }
	public User User { get; set; }
	public IList<Invoice> Invoices { get; set; }

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
	public DateTimeOffset DeletedAt { get; set; } = DateTimeOffset.MinValue;
}