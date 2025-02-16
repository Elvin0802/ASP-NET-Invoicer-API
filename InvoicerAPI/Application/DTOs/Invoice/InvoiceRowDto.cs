namespace InvoicerAPI.Application.DTOs.Invoice;

public class InvoiceRowDto
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Service { get; set; } = string.Empty;
	public decimal Quantity { get; set; }
	public decimal Amount { get; set; }
	public decimal Sum { get; set; }
}
