using System.Security.Claims;

namespace InvoicerAPI.Core.Interfaces.Auth;

public interface IJwtService
{
	string GenerateSecurityToken(
		Guid id,
		string email,
		IEnumerable<string> roles,
		IEnumerable<Claim> userClaims
		);
}
