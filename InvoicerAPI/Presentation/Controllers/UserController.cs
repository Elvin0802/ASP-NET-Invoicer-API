using InvoicerAPI.Application.DTOs.Auth;
using InvoicerAPI.Application.DTOs.Users;
using InvoicerAPI.Core.Entities;
using InvoicerAPI.Core.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoicerAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;
	private readonly IJwtService _jwtService;

	public UserController(UserManager<User> userManager,
							SignInManager<User> signInManager,
							IJwtService jwtService)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_jwtService = jwtService;
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthTokenDto>> Login([FromBody] LoginUserRequestDto request)
	{
		try
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user is null) return NotFound("User not found. Login failed.");

			var canSignIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

			if (!canSignIn.Succeeded) return Unauthorized("Something went wrong, login failed.");

			return Ok(await GenerateToken(user));
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpPost("register")]
	public async Task<ActionResult<AuthTokenDto>> Register([FromBody] RegisterUserRequestDto request)
	{
		try
		{
			var exisitingUser = await _userManager.FindByEmailAsync(request.Email);

			if (exisitingUser is not null) return Conflict("User already exsist, register failed.");

			var user = new User
			{
				Email = request.Email,
				UserName = request.Email,
				RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
				Name = request.Name,
				Address = request.Address,
				PhoneNumber = request.PhoneNumber,
				CreatedAt = DateTimeOffset.UtcNow,
				UpdatedAt = DateTimeOffset.UtcNow,
				DeletedAt = DateTimeOffset.MinValue
			};

			var result = await _userManager.CreateAsync(user, request.Password);

			if (!result.Succeeded) return BadRequest(result.Errors);

			return Ok(await GenerateToken(user));
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	private async Task<AuthTokenDto> GenerateToken(User user)
	{
		var roles = await _userManager.GetRolesAsync(user);

		var userClaims = await _userManager.GetClaimsAsync(user);

		var accessToken = _jwtService.GenerateSecurityToken(user.Id, user.Email!, roles, userClaims);

		var refreshToken = Guid.NewGuid().ToString("N").ToLower();

		user.RefreshToken = refreshToken;

		await _userManager.UpdateAsync(user);

		return new AuthTokenDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<AuthTokenDto>> Refresh([FromBody] RefreshTokenRequestDto request)
	{
		try
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

			if (user is null) return Unauthorized("Refresh is failed.");

			return Ok(await GenerateToken(user));
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpPut("update-info")]
	public async Task<ActionResult<AuthTokenDto>> UpdateUserInfo([FromBody] UpdateUserInfoRequestDto request)
	{
		try
		{
			if (request is null || request.Id == Guid.Empty) return BadRequest(nameof(request));

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

			if (user is null) return Unauthorized("User not found. Update User Info is failed.");

			if (!(string.IsNullOrEmpty(request.Name)) && request.Name != user.Name)
				user.Name = request.Name;

			if (!(string.IsNullOrEmpty(request.Address)) && request.Address != user.Address)
				user.Address = request.Address;

			if (!(string.IsNullOrEmpty(request.Email)) && request.Email != user.Email)
			{
				user.Email = request.Email;
				user.UserName = request.Email;
				//user.NormalizedUserName = request.Email.ToUpper();
				//	user.NormalizedEmail = request.Email.ToUpper();
			}

			if (!(string.IsNullOrEmpty(request.PhoneNumber)) && request.PhoneNumber != user.PhoneNumber)
				user.PhoneNumber = request.PhoneNumber;

			await _userManager.UpdateAsync(user);

			return Ok(await GenerateToken(user));
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpPut("change-password")]
	public async Task<ActionResult<AuthTokenDto>> ChangeUserPassword([FromBody] ChangeUserPasswordRequestDto request)
	{
		try
		{
			if (request is null
				|| request.Id == Guid.Empty
				|| string.IsNullOrEmpty(request.OldPassword)
				|| string.IsNullOrEmpty(request.NewPassword))
			{
				return BadRequest(nameof(request));
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

			if (user is null)
				return Unauthorized("User not found. Change User Password is failed.");

			if (request.OldPassword == request.NewPassword
				|| user.PasswordHash != request.OldPassword)
			{
				BadRequest(nameof(request));
			}

			await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

			await _userManager.UpdateAsync(user);

			return Ok(await GenerateToken(user));
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}


	[HttpDelete("{id}/delete")]
	public async Task<ActionResult> DeleteUser(Guid id)
	{
		try
		{
			if (id == Guid.Empty) return BadRequest("Id is Empty. Delete User is failed.");

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (user is null) return Unauthorized("User not found. Delete User is failed.");

			await _userManager.DeleteAsync(user);

			return Ok();
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}
}