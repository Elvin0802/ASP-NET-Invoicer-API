using InvoicerAPI.Application.DTOs.Providers;
using InvoicerAPI.Core.Interfaces.Providers;

namespace InvoicerAPI.Infrastructure.Providers;

public class RequestUserProvider : IRequestUserProvider
{
	private readonly HttpContext _httpContext;

	public RequestUserProvider(IHttpContextAccessor httpContext)
	{
		_httpContext = httpContext.HttpContext!;
	}

	public UserInfoDto? GetUserInfo()
	{
		if (!_httpContext.User.Claims.Any())
			return null;

		var id = _httpContext.User
							 .Claims
							 .First(c => c.Type == "userId")
							 .Value;

		var email = _httpContext.User
								.Identity!
								.Name;

		return new UserInfoDto(id, email!);
	}
}