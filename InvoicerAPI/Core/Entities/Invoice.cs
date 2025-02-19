using InvoicerAPI.Core.Enums;

namespace InvoicerAPI.Core.Entities;

public class Invoice
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Customer Customer { get; set; }
	public User User { get; set; }

	/// <summary>
	/// Start Date of the work period.
	/// </summary>
	public DateTimeOffset StartDate { get; set; }

	/// <summary>
	/// End Date of the work period.
	/// </summary>
	public DateTimeOffset EndDate { get; set; }
	public IList<InvoiceRow> Rows { get; set; } = null;

	/// <summary>
	/// Total Sum of Rows.
	/// </summary>
	public decimal TotalSum { get; set; } = 0;
	public string? Comment { get; set; } = string.Empty;
	public InvoiceStatus Status { get; set; } = InvoiceStatus.Created;

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