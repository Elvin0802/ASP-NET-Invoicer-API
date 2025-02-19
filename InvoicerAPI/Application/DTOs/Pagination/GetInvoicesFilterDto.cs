using InvoicerAPI.Core.Enums;

namespace InvoicerAPI.Application.DTOs.Pagination;

public class GetInvoicesFilterDto : BaseFilterDto
{
	public string? SearchText { get; set; }
	public InvoiceStatus? Status { get; set; }
	public DateTimeOffset? StartDate { get; set; }
	public DateTimeOffset? EndDate { get; set; }
}

