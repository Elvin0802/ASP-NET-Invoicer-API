namespace InvoicerAPI.Application.DTOs.Providers;

public class UserInfoDto
{
	public Guid Id { get; }
	public string UserEmail { get; }

	public UserInfoDto(Guid id, string userEmail)
	{
		Id = id;
		UserEmail = userEmail;
	}
}
