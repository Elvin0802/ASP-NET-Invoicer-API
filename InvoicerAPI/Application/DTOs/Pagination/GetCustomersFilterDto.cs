namespace InvoicerAPI.Application.DTOs.Pagination;

public class GetCustomersFilterDto : BaseFilterDto
{
	public string? SearchName { get; set; }
	public string? SearchEmail { get; set; }
	public string? SearchPhoneNumber { get; set; }
}
