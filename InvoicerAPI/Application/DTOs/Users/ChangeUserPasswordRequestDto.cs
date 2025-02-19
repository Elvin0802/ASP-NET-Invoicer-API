using System.ComponentModel.DataAnnotations;

namespace InvoicerAPI.Application.DTOs.Users;

public class ChangeUserPasswordRequestDto
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	[MinLength(8)]
	public string OldPassword { get; set; }

	[Required]
	[MinLength(8)]
	public string NewPassword { get; set; }
}
