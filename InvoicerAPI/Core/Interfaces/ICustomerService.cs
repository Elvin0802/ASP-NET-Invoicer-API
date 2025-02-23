using InvoicerAPI.Application.DTOs.Customers;
using InvoicerAPI.Application.DTOs.Pagination;

namespace InvoicerAPI.Core.Interfaces;

/// <summary>
/// Service interface for managing Customers.
/// </summary>
public interface ICustomerService
{
	/// <summary>
	/// Creates a new Customer.
	/// </summary>
	/// <param name="customerDto">The data transfer object containing Customer details.</param>
	/// <returns>The created Customer as a data transfer object.</returns>
	Task<CustomerDto> CreateCustomerAsync(Guid userId, BaseCustomerDto customerDto);

	/// <summary>
	/// Edites an existing Customer.
	/// </summary>
	/// <param name="id">id of customer</param>
	/// <param name="customerDto">The data transfer object containing edited Customer details.</param>
	/// <returns>The Edited Customer as a data transfer object.</returns>
	Task<CustomerDto> EditCustomerAsync(Guid id, BaseCustomerDto customerDto);

	/// <summary>
	/// Deletes a Customer by Id.
	/// </summary>
	/// <param name="CustomerId">The unique identifier of the Customer to delete.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<bool> DeleteCustomerAsync(Guid CustomerId);

	/// <summary>
	/// Archives a Customer by Id.
	/// </summary>
	/// <param name="id">The unique identifier of the Customer to archive.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<bool> ArchiveCustomerAsync(Guid id);

	/// <summary>
	/// Retrieves a list of all Customers.
	/// </summary>
	/// <returns>A list of Customers as data transfer objects.</returns>
	Task<PaginatedListDto<CustomerDto>> GetCustomersAsync(GetCustomersFilterDto filterDto);

	/// <summary>
	/// Retrieves a Customer by Id.
	/// </summary>
	/// <param name="CustomerId">The unique identifier of the Customer.</param>
	/// <returns>The Customer as a data transfer object, or null if not found.</returns>
	Task<CustomerDto> GetCustomerByIdAsync(Guid id);
}

