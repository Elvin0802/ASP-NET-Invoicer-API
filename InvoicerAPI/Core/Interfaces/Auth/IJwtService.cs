using System.Security.Claims;

namespace InvoicerAPI.Core.Interfaces.Auth;

public interface IJwtService
{
	string GenerateSecurityToken(
		string id,
		string email,
		IEnumerable<string> roles,
		IEnumerable<Claim> userClaims
		);
}
