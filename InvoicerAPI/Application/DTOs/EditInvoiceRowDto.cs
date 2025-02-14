namespace InvoicerAPI.Application.DTOs;

public class EditInvoiceRowDto
{
	public Guid Id { get; set; }
	public string Service { get; set; } = string.Empty;
	public decimal Quantity { get; set; }
	public decimal Amount { get; set; }
}
