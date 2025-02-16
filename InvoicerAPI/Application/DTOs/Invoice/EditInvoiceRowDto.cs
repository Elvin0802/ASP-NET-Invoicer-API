namespace InvoicerAPI.Application.DTOs.Invoice;

public class EditInvoiceRowDto
{
	public Guid Id { get; set; }
	public string Service { get; set; } = string.Empty;
	public decimal Quantity { get; set; }
	public decimal Amount { get; set; }
}
