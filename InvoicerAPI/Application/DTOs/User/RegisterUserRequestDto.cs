﻿using System.ComponentModel.DataAnnotations;

namespace InvoicerAPI.Application.DTOs.User;

public class RegisterUserRequestDto
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[MinLength(8)]
	public string Password { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public string Address { get; set; }

	[Required]
	public string PhoneNumber { get; set; }
}