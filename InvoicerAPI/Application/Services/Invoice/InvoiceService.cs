using InvoicerAPI.Application.DTOs.Invoice;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Entities;
using InvoicerAPI.Core.Enums;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InvoicerAPI.Application.Services.Invoice;

public class InvoiceService : IInvoiceService
{
	private readonly InvoicerDbContext _context;

	public InvoiceService(InvoicerDbContext context)
	{
		_context = context;
	}

	public Task<bool> ArchiveInvoiceAsync(Guid id)
	{
		var invoice = _context.Invoices.Include(i => i.Rows)
									   .FirstOrDefault(i => i.Id == id);

		if (invoice is null)
			return Task.FromResult(false);

		invoice.DeletedAt = DateTimeOffset.UtcNow;

		_context.Invoices.Update(invoice);
		_context.SaveChanges();

		return Task.FromResult(true);
	}

	public Task<InvoiceDto> ChangeInvoiceStatusAsync(Guid id, InvoiceStatus newStatus)
	{
		var invoice = _context.Invoices.Include(i => i.Rows)
									   .FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(invoice);

		invoice.Status = newStatus;
		invoice.UpdatedAt = DateTimeOffset.UtcNow;

		_context.SaveChanges();

		var invoiceDto = new InvoiceDto()
		{
			Id = invoice.Id,
			CustomerId = invoice.CustomerId,
			StartDate = invoice.StartDate,
			EndDate = invoice.EndDate,
			TotalSum = invoice.TotalSum,
			Comment = invoice.Comment,
			Status = invoice.Status,
			CreatedAt = invoice.CreatedAt,
			UpdatedAt = invoice.UpdatedAt,

			Rows = ConvertInvoiceRowToInvoiceRowDto(invoice.Rows),
		};

		return Task.FromResult(invoiceDto);
	}

	public Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto)
	{
		var invoice = new Core.Entities.Invoice()
		{
			CreatedAt = DateTime.UtcNow,
			Status = InvoiceStatus.Created,
			StartDate = createInvoiceDto.StartDate,
			EndDate = createInvoiceDto.EndDate,
			Comment = createInvoiceDto.Comment,
		};

		invoice.UpdatedAt = invoice.CreatedAt;
		invoice.Rows = ConvertCreateInvoiceRowDtoToInvoiceRow(invoice.Id, createInvoiceDto.Rows);
		invoice.TotalSum = invoice.Rows.Sum(e => e.Sum);

		_context.Invoices.Add(invoice);
		_context.SaveChanges();

		var invoiceDto = new InvoiceDto()
		{
			Id = invoice.Id,
			CustomerId = invoice.CustomerId,
			StartDate = invoice.StartDate,
			EndDate = invoice.EndDate,
			TotalSum = invoice.TotalSum,
			Comment = invoice.Comment,
			Status = invoice.Status,
			CreatedAt = invoice.CreatedAt,
			UpdatedAt = invoice.UpdatedAt,

			Rows = ConvertInvoiceRowToInvoiceRowDto(invoice.Rows),
		};

		return Task.FromResult(invoiceDto);
	}

	public Task<bool> DeleteInvoiceAsync(Guid id)
	{
		var invoice = _context.Invoices.Include(i => i.Rows)
									   .FirstOrDefault(i => i.Id == id);

		if (invoice is null || invoice.Status != InvoiceStatus.Created)
			return Task.FromResult(false);

		_context.Invoices.Remove(invoice);
		_context.SaveChanges();

		return Task.FromResult(true);
	}

	public Task<InvoiceDto> EditInvoiceAsync(Guid id, EditInvoiceDto editInvoiceDto)
	{
		var invoice = _context.Invoices.Include(i => i.Rows)
									   .FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(invoice);

		invoice.StartDate = editInvoiceDto.StartDate;
		invoice.EndDate = editInvoiceDto.EndDate;
		invoice.Comment = editInvoiceDto.Comment;

		foreach (var row in invoice.Rows)
		{
			var t = editInvoiceDto.Rows.FirstOrDefault(x => x.Id == row.Id);

			if (t is null) continue;

			row.Quantity = t.Quantity;
			row.Amount = t.Amount;
			row.Service = t.Service;
			row.Sum = t.Quantity * t.Amount;
		}

		invoice.TotalSum = invoice.Rows.Sum(e => e.Sum);

		_context.Invoices.Update(invoice);
		_context.SaveChanges();

		var invoiceDto = new InvoiceDto()
		{
			Id = invoice.Id,
			CustomerId = invoice.CustomerId,
			StartDate = invoice.StartDate,
			EndDate = invoice.EndDate,
			TotalSum = invoice.TotalSum,
			Comment = invoice.Comment,
			Status = invoice.Status,
			CreatedAt = invoice.CreatedAt,
			UpdatedAt = invoice.UpdatedAt,

			Rows = ConvertInvoiceRowToInvoiceRowDto(invoice.Rows),
		};

		return Task.FromResult(invoiceDto);
	}

	public Task<InvoiceDto> GetInvoiceByIdAsync(Guid id)
	{
		var invoice = _context.Invoices.Include(i => i.Rows).FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(invoice);

		if (invoice.DeletedAt >= invoice.CreatedAt)
			throw new DeletedRowInaccessibleException();

		var dto = new InvoiceDto
		{
			Id = invoice.Id,
			CustomerId = invoice.CustomerId,
			StartDate = invoice.StartDate,
			EndDate = invoice.EndDate,
			TotalSum = invoice.TotalSum,
			Comment = invoice.Comment,
			Status = invoice.Status,
			CreatedAt = invoice.CreatedAt,
			UpdatedAt = invoice.UpdatedAt,
			Rows = ConvertInvoiceRowToInvoiceRowDto(invoice.Rows)
		};

		return Task.FromResult(dto);
	}

	public Task<PaginatedListDto<InvoiceDto>> GetInvoicesListAsync(GetInvoicesFilterDto filterDto)
	{
		var query = _context.Invoices.Include(i => i.Rows).AsQueryable();

		query = query.Where(i => i.CreatedAt > i.DeletedAt);

		if (!string.IsNullOrEmpty(filterDto.SearchText))
			query = query.Where(i => i.Comment!.Contains(filterDto.SearchText, StringComparison.CurrentCultureIgnoreCase));

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
		else
			query = query.OrderBy(i => i.CreatedAt);

		var totalCount = query.Count();

		var items = query.Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
						 .Take(filterDto.PageSize)
						 .ToList();

		var dtoList = items.Select(i => new InvoiceDto
		{
			Id = i.Id,
			CustomerId = i.CustomerId,
			StartDate = i.StartDate,
			EndDate = i.EndDate,
			TotalSum = i.TotalSum,
			Comment = i.Comment,
			Status = i.Status,
			CreatedAt = i.CreatedAt,
			UpdatedAt = i.UpdatedAt,
			Rows = ConvertInvoiceRowToInvoiceRowDto(i.Rows)
		}).ToList();

		return Task.FromResult(new PaginatedListDto<InvoiceDto>
		{
			Items = dtoList,
			PageNumber = filterDto.PageNumber,
			PageSize = filterDto.PageSize,
			TotalCount = totalCount
		});
	}

	#region Convert Methods

	private List<InvoiceRow> ConvertCreateInvoiceRowDtoToInvoiceRow(Guid invoiceId, IList<CreateInvoiceRowDto> list)
	{
		if (invoiceId == Guid.Empty || list is null)
			throw new ArgumentNullException(nameof(invoiceId));

		var rows = new List<InvoiceRow>();

		foreach (var row in list)
		{
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
