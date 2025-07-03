using InvoicerAPI.Application.DTOs.Customers;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Core.Interfaces.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerrAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClientsController : ControllerBase
{
	private readonly ICustomerService _service;
	private readonly IRequestUserProvider _userProvider;

	public ClientsController(ICustomerService service, IRequestUserProvider userProvider)
	{
		_service = service;
		_userProvider = userProvider;
	}

	[HttpPost("create")]
	public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] BaseCustomerDto dto)
	{
		try
		{
			var user = _userProvider.GetUserInfo();

			var customer = await _service.CreateCustomerAsync(user.Id, dto);

			return customer is null ? throw new Exception() : customer;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred. | Clients/create");
		}
	}

	[HttpGet]
	public async Task<ActionResult<PaginatedListDto<CustomerDto>>> GetCustomers([FromQuery] GetCustomersFilterDto dto)
	{
		try
		{
			var list = await _service.GetCustomersAsync(dto);

			return list is null ? NotFound() : list;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
	{
		try
		{
			var invoice = await _service.GetCustomerByIdAsync(id);

			return invoice is null ? NotFound() : invoice;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpDelete("{id}/delete")]
	public async Task<ActionResult> DeleteCustomer(Guid id)
	{
		try
		{
			return await _service.DeleteCustomerAsync(id) ? Ok() : NotFound();
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpDelete("{id}/archive")]
	public async Task<ActionResult> ArchiveCustomer(Guid id)
	{
		try
		{
			return await _service.ArchiveCustomerAsync(id) ? Ok() : NotFound();
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}

	[HttpPut("{id}/edit")]
	public async Task<ActionResult<CustomerDto>> EditCustomer(Guid id, [FromBody] BaseCustomerDto dto)
	{
		try
		{
			var customer = await _service.EditCustomerAsync(id, dto);

			return (customer is null) ? NotFound() : customer;
		}
		catch
		{
			return StatusCode(500, "An unexpected error occurred.");
		}
	}
}
