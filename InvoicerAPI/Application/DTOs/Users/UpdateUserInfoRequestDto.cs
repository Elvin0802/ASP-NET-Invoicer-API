using System.ComponentModel.DataAnnotations;

namespace InvoicerAPI.Application.DTOs.Users;

public class UpdateUserInfoRequestDto
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	public string? Address { get; set; }

	[Required]
	public string? PhoneNumber { get; set; }
}
