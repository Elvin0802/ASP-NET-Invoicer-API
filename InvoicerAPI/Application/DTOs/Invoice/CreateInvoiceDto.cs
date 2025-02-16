namespace InvoicerAPI.Application.DTOs.Invoice;

public class CreateInvoiceDto
{
	public DateTimeOffset StartDate { get; set; }
	public DateTimeOffset EndDate { get; set; }
	public IList<CreateInvoiceRowDto> Rows { get; set; }
	public string? Comment { get; set; }
}
