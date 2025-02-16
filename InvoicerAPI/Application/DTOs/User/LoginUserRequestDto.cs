using System.ComponentModel.DataAnnotations;

namespace InvoicerAPI.Application.DTOs.User;

public class LoginUserRequestDto
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[MinLength(8)]
	public string Password { get; set; }
}
