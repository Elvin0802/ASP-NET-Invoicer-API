

namespace InvoicerAPI.Core.Entities;

public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string? Address { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public string? PhoneNumber { get; set; }

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