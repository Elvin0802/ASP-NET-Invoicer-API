using FluentValidation;

namespace InvoicerAPI.Application.Validation.Extensions;

public static class ValidationExtensions
{
	public static IRuleBuilderOptions<T, string> CheckName<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		return ruleBuilder.NotEmpty()
						  .Matches(@"^[a-zA-Z]+$")
						  .WithMessage("Name must contain only letters.");
	}
	public static IRuleBuilderOptions<T, string> CheckAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		return ruleBuilder.NotEmpty()
						  .MinimumLength(6)
						  .WithMessage("Address must be at least 6 characters long.");
	}
	public static IRuleBuilderOptions<T, string> CheckPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		return ruleBuilder.NotEmpty()
						  .Matches(@"^\+994\s(10|50|51|55|70|77|99|12)\s\d{3}\s\d{2}\s\d{2}$")
						  .WithMessage("Phone number must be in format '+994 XX XXX XX XX' where XX is one of: 10, 50, 51, 55, 70, 77, 99, 12.");
	}
	public static IRuleBuilderOptions<T, string> CheckPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
	{
		return ruleBuilder.NotEmpty()
						  .MinimumLength(8)
						  .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$")
						  .WithMessage("Password must be at least 8 characters" +
									   " and contain at least one uppercase letter," +
									   " one lowercase letter, one number and" +
									   " one special character");
	}
}
