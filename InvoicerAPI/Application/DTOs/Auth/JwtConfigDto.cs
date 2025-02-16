namespace InvoicerAPI.Application.DTOs.Auth;

public class JwtConfigDto
{
	public string Secret { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
	public int Expiration { get; set; }
}