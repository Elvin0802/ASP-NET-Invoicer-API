using InvoicerAPI.Application.DTOs;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Enums;
using InvoicerAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoicerAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
	private readonly IInvoiceService _service;

	public InvoicesController(IInvoiceService service)
	{
		_service = service;
	}

	[HttpPost("create")]
	public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] CreateInvoiceDto dto)
	{
		try
		{
			var invoice = await _service.CreateInvoiceAsync(dto);

			return invoice is null ? NotFound() : invoice;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpGet]
	public async Task<ActionResult<PaginatedListDto<InvoiceDto>>> GetInvoices([FromQuery] GetInvoicesFilterDto dto)
	{
		try
		{
			var list = await _service.GetInvoicesListAsync(dto);

			return list is null ? NotFound() : list;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpGet("/invoice")]
	public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid id)
	{
		try
		{
			var invoice = await _service.GetInvoiceByIdAsync(id);

			return invoice is null ? NotFound() : invoice;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpPatch("{id}/change-status")]
	public async Task<ActionResult<InvoiceDto>> ChangeInvoiceStatus(Guid id, [FromBody] InvoiceStatus newStatus)
	{
		try
		{
			var invoice = await _service.ChangeInvoiceStatusAsync(id, newStatus);

			return invoice is null ? NotFound() : invoice;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpPut("{id}/edit")]
	public async Task<ActionResult<InvoiceDto>> EditInvoice(Guid id, [FromBody] EditInvoiceDto dto)
	{
		try
		{
			var invoice = await _service.EditInvoiceAsync(id, dto);

			return (await _service.EditInvoiceAsync(id, dto) is null) ? NotFound() : invoice;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpDelete("{id}/delete")]
	public async Task<ActionResult> DeleteInvoice(Guid id)
	{
		try
		{
			return await _service.DeleteInvoiceAsync(id) ? Ok() : NotFound();
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpDelete("{id}/archive")]
	public async Task<ActionResult> ArchiveInvoice(Guid id)
	{
		try
		{
			return await _service.ArchiveInvoiceAsync(id) ? Ok() : NotFound();
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}
}
