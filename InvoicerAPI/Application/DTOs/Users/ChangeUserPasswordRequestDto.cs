namespace InvoicerAPI.Application.DTOs.Users;

public class ChangeUserPasswordRequestDto
{
	public Guid Id { get; set; }

	public string OldPassword { get; set; }

	public string NewPassword { get; set; }
}
