﻿namespace InvoicerAPI.Application.DTOs.Users;

public class RegisterUserRequestDto
{
	public string Email { get; set; }
	public string Password { get; set; }
	public string Name { get; set; }
	public string Address { get; set; }
	public string PhoneNumber { get; set; }
}