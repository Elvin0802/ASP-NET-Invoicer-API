using InvoicerAPI.Application.DTOs.Invoices;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Entities;
using InvoicerAPI.Core.Enums;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InvoicerAPI.Application.Services;

public class InvoiceService : IInvoiceService
{
	private readonly InvoicerDbContext _context;

	public InvoiceService(InvoicerDbContext context)
	{
		_context = context;
	}

	public Task<bool> ArchiveInvoiceAsync(Guid id)
	{
		var invoice = _context.Invoices.FirstOrDefault(i => i.Id == id);

		if (invoice is null)
			return Task.FromResult(false);

		invoice.DeletedAt = DateTimeOffset.UtcNow;

		_context.Invoices.Update(invoice);
		_context.SaveChanges();

		return Task.FromResult(true);
	}

	public Task<InvoiceDto> ChangeInvoiceStatusAsync(Guid id, InvoiceStatus newStatus)
	{
		var invoice = _context.Invoices
								.Include(i => i.User)
								.Include(i => i.Rows)
								.Include(i => i.Customer)
								.FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(invoice);

		if ((int)newStatus < 0 || (int)newStatus > 5)
			throw new ArgumentOutOfRangeException(nameof(newStatus));

		invoice.Status = newStatus;
		invoice.UpdatedAt = DateTimeOffset.UtcNow;

		_context.SaveChanges();

		return Task.FromResult(ConvertInvoiceToInvoiceDto(invoice));
	}

	public Task<InvoiceDto> CreateInvoiceAsync(Guid userId, CreateInvoiceDto createInvoiceDto)
	{
		var invoice = ConvertCreateInvoiceDtoToInvoice(userId, createInvoiceDto);

		invoice = _context.Invoices.Add(invoice).Entity;

		_context.SaveChanges();

		return Task.FromResult(ConvertInvoiceToInvoiceDto(invoice));
	}

	public Task<bool> DeleteInvoiceAsync(Guid id)
	{
		var invoice = _context.Invoices.FirstOrDefault(i => i.Id == id);

		if (invoice is null || invoice.Status != InvoiceStatus.Created)
			return Task.FromResult(false);

		_context.Invoices.Remove(invoice);
		_context.SaveChanges();

		return Task.FromResult(true);
	}

	public Task<InvoiceDto> EditInvoiceAsync(Guid id, EditInvoiceDto editInvoiceDto)
	{
		var invoice = _context.Invoices
							.Include(i => i.User)
							.Include(i => i.Customer)
							.Include(i => i.Rows)
							.FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(invoice);

		invoice.StartDate = editInvoiceDto.StartDate;
		invoice.EndDate = editInvoiceDto.EndDate;
		invoice.Comment = editInvoiceDto.Comment;

		foreach (var row in invoice.Rows)
		{
			var t = editInvoiceDto.Rows.FirstOrDefault(x => x.Id == row.Id);

			if (t is null) continue;

			if (t.Quantity < 0 || t.Amount < 0)
				throw new ArgumentOutOfRangeException(nameof(t));

			row.Quantity = t.Quantity;
			row.Amount = t.Amount;
			row.Service = t.Service;
			row.Sum = t.Quantity * t.Amount;
		}

		invoice.TotalSum = invoice.Rows.Sum(e => e.Sum);

		_context.Invoices.Update(invoice);
		_context.SaveChanges();

		return Task.FromResult(ConvertInvoiceToInvoiceDto(invoice));
	}

	public Task<InvoiceDto> GetInvoiceByIdAsync(Guid id)
	{
		var invoice = _context.Invoices
							.Include(i => i.User)
							.Include(i => i.Customer)
							.Include(i => i.Rows)
							.FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(invoice);

		if (invoice.DeletedAt >= invoice.CreatedAt)
			throw new DeletedRowInaccessibleException();

		return Task.FromResult(ConvertInvoiceToInvoiceDto(invoice));
	}

	public Task<PaginatedListDto<InvoiceDto>> GetInvoicesListAsync(GetInvoicesFilterDto filterDto)
	{
		var query = _context.Invoices
							.Include(i => i.User)
							.Include(i => i.Customer)
							.Include(i => i.Rows)
							.AsQueryable();

		query = query.Where(i => i.CreatedAt > i.DeletedAt);

		if (!string.IsNullOrEmpty(filterDto.SearchText))
			query = query.Where(i => i.Comment!
									  .Contains(filterDto.SearchText,
												StringComparison.CurrentCultureIgnoreCase));

		if (filterDto.Status.HasValue)
			query = query.Where(i => i.Status == filterDto.Status.Value);

		if (filterDto.StartDate.HasValue)
			query = query.Where(i => i.StartDate >= filterDto.StartDate.Value);

		if (filterDto.EndDate.HasValue)
			query = query.Where(i => i.EndDate <= filterDto.EndDate.Value);

		if (!string.IsNullOrEmpty(filterDto.SortBy))
		{
			query = filterDto.IsDescending
					? query.OrderByDescending(e => EF.Property<object>(e, filterDto.SortBy))
					: query.OrderBy(e => EF.Property<object>(e, filterDto.SortBy));
		}
		else query = query.OrderBy(i => i.CreatedAt);

		var totalCount = query.Count();

		var items = query.Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
						 .Take(filterDto.PageSize)
						 .ToList();

		var dtoList = items.Select(ConvertInvoiceToInvoiceDto)
						   .ToList();

		return Task.FromResult(new PaginatedListDto<InvoiceDto>
		{
			Items = dtoList,
			PageNumber = filterDto.PageNumber,
			PageSize = filterDto.PageSize,
			TotalCount = totalCount
		});
	}


	#region Convert Methods

	/// <summary>
	/// Convert create invoice dto to invoice.
	/// </summary>
	/// <param name="userId">id of user</param>
	/// <param name="createInvoiceDto">Create Invoice Dto for converting to Invoice.</param>
	/// <returns>Object of Invoice</returns>
	public Invoice ConvertCreateInvoiceDtoToInvoice(Guid userId, CreateInvoiceDto createInvoiceDto)
	{
		var user = _context.Users.Find(userId)
					?? throw new KeyNotFoundException("User Not Found.");

		var customer = _context.Customers.Find(createInvoiceDto.CustomerId)
						?? throw new KeyNotFoundException("Customer Not Found.");

		var invoice = new Invoice();

		invoice.CustomerId = customer.Id;

		invoice.UserId = user.Id;

		invoice.CreatedAt = DateTime.UtcNow;

		invoice.UpdatedAt = invoice.CreatedAt;

		invoice.Status = InvoiceStatus.Created;

		invoice.StartDate = createInvoiceDto.StartDate;

		invoice.EndDate = createInvoiceDto.EndDate;

		invoice.Comment = createInvoiceDto.Comment;

		invoice.Rows = ConvertCreateInvoiceRowDtoToInvoiceRow(invoice.Id, createInvoiceDto.Rows);

		invoice.TotalSum = invoice.Rows.Sum(e => e.Sum);

		return invoice;
	}

	/// <summary>
	/// Convert invoice to invoice dto.
	/// </summary>
	/// <param name="invoice">invoice for converting to dto.</param>
	/// <returns>Object of InvoiceDto</returns>
	public InvoiceDto ConvertInvoiceToInvoiceDto(Invoice invoice)
	{
		return new InvoiceDto()
		{
			Id = invoice.Id,
			CustomerId = invoice.Customer.Id,
			StartDate = invoice.StartDate,
			EndDate = invoice.EndDate,
			TotalSum = invoice.TotalSum,
			Comment = invoice.Comment,
			Status = invoice.Status,
			CreatedAt = invoice.CreatedAt,
			UpdatedAt = invoice.UpdatedAt,

			Rows = ConvertInvoiceRowToInvoiceRowDto(invoice.Rows),
		};
	}

	/// <summary>
	/// Convert create invoice row dto's to invoice row's.
	/// </summary>
	/// <param name="invoiceId">ivoice id for setting parent invoice id of all invoice row's.</param>
	/// <param name="list"></param>
	/// <returns>List of InvoiceRow.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	private List<InvoiceRow> ConvertCreateInvoiceRowDtoToInvoiceRow(Guid invoiceId,
																	IList<CreateInvoiceRowDto> list)
	{
		if (invoiceId == Guid.Empty || list is null)
			throw new ArgumentNullException(nameof(invoiceId));

		var rows = new List<InvoiceRow>();

		foreach (var row in list)
		{
			if (row.Quantity < 0 || row.Amount < 0)
				throw new ArgumentOutOfRangeException(nameof(row));

			rows.Add(new InvoiceRow()
			{
				InvoiceId = invoiceId,
				Service = row.Service,
				Quantity = row.Quantity,
				Amount = row.Amount,
				Sum = row.Quantity * row.Amount,
			});
		}
		return rows;
	}

	/// <summary>
	/// Convert invoice row's to invoice row dto's.
	/// </summary>
	/// <param name="list">list of invoice row.</param>
	/// <returns>List of InvoiceRowDto.</returns>
	private List<InvoiceRowDto> ConvertInvoiceRowToInvoiceRowDto(IList<InvoiceRow> list)
	{
		ArgumentNullException.ThrowIfNull(list);

		var rows = new List<InvoiceRowDto>();

		foreach (var row in list)
		{
			rows.Add(new InvoiceRowDto()
			{
				Id = row.Id,
				Service = row.Service,
				Quantity = row.Quantity,
				Amount = row.Amount,
				Sum = row.Sum,
			});
		}
		return rows;
	}

	#endregion
}
