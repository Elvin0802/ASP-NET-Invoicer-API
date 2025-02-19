namespace InvoicerAPI.Application.DTOs.Invoices;

public class CreateInvoiceDto
{
	public Guid CustomerId { get; set; }
	public Guid UserId { get; set; }
	public DateTimeOffset StartDate { get; set; }
	public DateTimeOffset EndDate { get; set; }
	public IList<CreateInvoiceRowDto> Rows { get; set; }
	public string? Comment { get; set; }
}
