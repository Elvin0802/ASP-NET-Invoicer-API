namespace InvoicerAPI.Core.Entities;

public class InvoiceRow
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid InvoiceId { get; set; }
	public Invoice Invoice { get; set; }

	/// <summary>
	/// Name of work.
	/// </summary>
	public string Service { get; set; }

	/// <summary>
	/// Quantity of work done.
	/// </summary>
	public decimal Quantity { get; set; }

	/// <summary>
	/// Amount of work.
	/// </summary>
	public decimal Amount { get; set; }

	/// <summary>
	/// Total amount ~~= sum | Quantity * Amount = Sum
	/// </summary>
	public decimal Sum { get; set; }
}