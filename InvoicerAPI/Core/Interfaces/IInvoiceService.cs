using InvoicerAPI.Application.DTOs.Invoice;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Enums;

namespace InvoicerAPI.Core.Interfaces;

/// <summary>
/// Main Service of Invoice.
/// </summary>
public interface IInvoiceService
{
	/// <summary>
	/// Creates a new invoice.
	/// </summary>
	/// <param name="invoiceDto">The invoice data transfer object.</param>
	/// <returns>The created invoice dto.</returns>
	Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto);

	/// <summary>
	/// Edits an existing invoice.
	/// Only invoices with status 'Created' can be edited.
	/// </summary>
	/// <param name="id">The invoice ID.</param>
	/// <param name="invoiceDto">The updated invoice data transfer object.</param>
	/// <returns>The updated invoice dto.</returns>
	Task<InvoiceDto> EditInvoiceAsync(Guid id, EditInvoiceDto editInvoiceDto);

	/// <summary>
	/// Changes the status of an invoice.
	/// </summary>
	/// <param name="id">The invoice ID.</param>
	/// <param name="newStatus">The new status to set.</param>
	/// <returns>The updated invoice with the new status.</returns>
	Task<InvoiceDto> ChangeInvoiceStatusAsync(Guid id, InvoiceStatus newStatus);

	/// <summary>
	/// Deletes an invoice (Hard delete).
	/// Only invoices with status 'Created' can be deleted.
	/// </summary>
	/// <param name="id">The invoice ID.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<bool> DeleteInvoiceAsync(Guid id);

	/// <summary>
	/// Archives an invoice (Soft delete).
	/// </summary>
	/// <param name="id">The invoice ID.</param>
	/// <returns>The archived invoice dto.</returns>
	Task<bool> ArchiveInvoiceAsync(Guid id);

	/// <summary>
	/// Gets a list of invoices with pagination, filters, and sorting.
	/// </summary>
	/// <param name="filterDto">The filter and pagination dto.</param>
	/// <returns>A paginated list of invoices.</returns>
	Task<PaginatedListDto<InvoiceDto>> GetInvoicesListAsync(GetInvoicesFilterDto filterDto);

	/// <summary>
	/// Gets the dto of a specific invoice by ID.
	/// </summary>
	/// <param name="id">The invoice ID.</param>
	/// <returns>The dto of the requested invoice.</returns>
	Task<InvoiceDto> GetInvoiceByIdAsync(Guid id);
}
