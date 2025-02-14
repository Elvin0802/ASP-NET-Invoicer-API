using InvoicerAPI.Core.Enums;

namespace InvoicerAPI.Application.DTOs;

public class InvoiceDto
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid CustomerId { get; set; }
	public DateTimeOffset StartDate { get; set; }
	public DateTimeOffset EndDate { get; set; }
	public List<InvoiceRowDto> Rows { get; set; } = new();
	public decimal TotalSum { get; set; }
	public string? Comment { get; set; }
	public InvoiceStatus Status { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
}
