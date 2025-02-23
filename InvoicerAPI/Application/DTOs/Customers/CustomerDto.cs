namespace InvoicerAPI.Application.DTOs.Customers;

public class CustomerDto : BaseCustomerDto
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid UserId { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
}
