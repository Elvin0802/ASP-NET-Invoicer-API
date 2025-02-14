using InvoicerAPI.Core.Enums;

namespace InvoicerAPI.Application.DTOs.Pagination;

public class GetInvoicesFilterDto
{
	public string? SearchText { get; set; }
	public InvoiceStatus? Status { get; set; }
	public DateTimeOffset? StartDate { get; set; }
	public DateTimeOffset? EndDate { get; set; }
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public string? SortBy { get; set; }
	public bool IsDescending { get; set; }
}

