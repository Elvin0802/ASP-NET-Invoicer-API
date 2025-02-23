using FluentValidation;
using InvoicerAPI.Application.DTOs.Users;
using InvoicerAPI.Application.Validation.Extensions;

namespace InvoicerAPI.Application.Validation.Validators.Users;

public class UpdateUserInfoRequestValidator : AbstractValidator<UpdateUserInfoRequestDto>
{
	public UpdateUserInfoRequestValidator()
	{
		RuleFor(e => e.Name).CheckName();
		RuleFor(e => e.Email).EmailAddress().NotEmpty();
		RuleFor(e => e.PhoneNumber).CheckPhoneNumber();
		RuleFor(e => e.Address).CheckAddress();
	}
}
