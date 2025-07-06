using InvoicerAPI.Application.DTOs.Invoices;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Enums;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Core.Interfaces.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoicerAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InvoicesController : ControllerBase
{
	private readonly IInvoiceService _service;
	private readonly IRequestUserProvider _userProvider;

	public InvoicesController(IInvoiceService service, IRequestUserProvider userProvider)
	{
		_service = service;
		_userProvider = userProvider;
	}

	[HttpPost("create")]
	public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] CreateInvoiceDto dto)
	{
		try
		{
			var user = _userProvider.GetUserInfo();

			var invoice = await _service.CreateInvoiceAsync(user!.Id, dto);

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

	[HttpGet("{id}")]
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

	[HttpPatch("{id}/change-status/{newStatus}")]
	public async Task<ActionResult<InvoiceDto>> ChangeInvoiceStatus(Guid id, int newStatus)
	{
		try
		{
			var invoice = await _service.ChangeInvoiceStatusAsync(id, (InvoiceStatus)newStatus);

			return invoice is null ? NotFound() : Ok(invoice);
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

			return (invoice is null) ? NotFound() : invoice;
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
