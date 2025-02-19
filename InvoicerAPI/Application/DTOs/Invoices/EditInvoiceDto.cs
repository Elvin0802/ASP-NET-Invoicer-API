namespace InvoicerAPI.Application.DTOs.Invoices;

public class EditInvoiceDto
{
	public DateTimeOffset StartDate { get; set; }
	public DateTimeOffset EndDate { get; set; }
	public string? Comment { get; set; }
	public List<EditInvoiceRowDto> Rows { get; set; }
}
