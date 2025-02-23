using FluentValidation;
using InvoicerAPI.Application.DTOs.Users;
using InvoicerAPI.Application.Validation.Extensions;

namespace InvoicerAPI.Application.Validation.Validators.Users;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequestDto>
{
	public RegisterUserRequestValidator()
	{
		RuleFor(e => e.Email).EmailAddress().NotEmpty();
		RuleFor(e => e.Name).CheckName();
		RuleFor(e => e.Address).CheckAddress();
		RuleFor(e => e.PhoneNumber).CheckPhoneNumber();
		RuleFor(e => e.Password).CheckPassword();
	}
}
