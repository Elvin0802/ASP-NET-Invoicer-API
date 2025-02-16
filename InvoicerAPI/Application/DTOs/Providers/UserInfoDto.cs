namespace InvoicerAPI.Application.DTOs.Providers;

public class UserInfoDto
{
	public string Id { get; }
	public string UserEmail { get; }

	public UserInfoDto(string id, string userEmail)
	{
		Id = id;
		UserEmail = userEmail;
	}
}
