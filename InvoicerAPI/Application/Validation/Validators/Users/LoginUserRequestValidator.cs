using FluentValidation;
using InvoicerAPI.Application.DTOs.Users;
using InvoicerAPI.Application.Validation.Extensions;

namespace InvoicerAPI.Application.Validation.Validators.Users;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequestDto>
{
	public LoginUserRequestValidator()
	{
		RuleFor(e => e.Email).EmailAddress().NotEmpty();
		RuleFor(e => e.Password).CheckPassword();
	}
}
