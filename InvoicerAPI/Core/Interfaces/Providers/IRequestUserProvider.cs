using InvoicerAPI.Application.DTOs.Providers;

namespace InvoicerAPI.Core.Interfaces.Providers;

public interface IRequestUserProvider
{
	UserInfoDto? GetUserInfo();
}
