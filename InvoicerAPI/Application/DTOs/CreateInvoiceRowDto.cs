﻿namespace InvoicerAPI.Application.DTOs;

public class CreateInvoiceRowDto
{
	public string Service { get; set; }
	public decimal Quantity { get; set; }
	public decimal Amount { get; set; }
}
