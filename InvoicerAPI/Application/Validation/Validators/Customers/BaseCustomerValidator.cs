using FluentValidation;
using InvoicerAPI.Application.DTOs.Customers;
using InvoicerAPI.Application.Validation.Extensions;

namespace InvoicerAPI.Application.Validation.Validators.Customers;

public class BaseCustomerValidator : AbstractValidator<BaseCustomerDto>
{
	public BaseCustomerValidator()
	{
		RuleFor(e => e.Name).CheckName();
		RuleFor(e => e.Email).EmailAddress().NotEmpty();
		RuleFor(e => e.PhoneNumber).CheckPhoneNumber();
	}
}
