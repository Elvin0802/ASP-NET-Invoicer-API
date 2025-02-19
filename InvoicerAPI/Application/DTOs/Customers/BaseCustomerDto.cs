namespace InvoicerAPI.Application.DTOs.Customers;

public class BaseCustomerDto
{
	public Guid UserId { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
}
