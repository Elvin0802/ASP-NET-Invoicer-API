using FluentValidation;
using InvoicerAPI.Application.DTOs.Users;
using InvoicerAPI.Application.Validation.Extensions;

namespace InvoicerAPI.Application.Validation.Validators.Users;

public class ChangeUserPasswordRequestValidator : AbstractValidator<ChangeUserPasswordRequestDto>
{
	public ChangeUserPasswordRequestValidator()
	{
		RuleFor(e => e.OldPassword).CheckPassword();
		RuleFor(e => e.NewPassword).CheckPassword();
	}
}
