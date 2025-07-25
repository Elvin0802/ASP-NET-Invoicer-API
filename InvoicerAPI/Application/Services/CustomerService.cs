﻿using InvoicerAPI.Application.DTOs.Customers;
using InvoicerAPI.Application.DTOs.Pagination;
using InvoicerAPI.Core.Entities;
using InvoicerAPI.Core.Interfaces;
using InvoicerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InvoicerAPI.Application.Services;

public class CustomerService : ICustomerService
{
	private readonly InvoicerDbContext _context;
	private readonly IInvoiceService _service;

	public CustomerService(InvoicerDbContext context, IInvoiceService service)
	{
		_context = context;
		_service = service;
	}

	public Task<CustomerDto> CreateCustomerAsync(Guid userId, BaseCustomerDto customerDto)
	{
		var user = _context.Users.Find(userId)
					?? throw new KeyNotFoundException("User Not Found.");

		var customer = new Customer()
		{
			Name = customerDto.Name,
			Email = customerDto.Email,
			PhoneNumber = customerDto.PhoneNumber,
			CreatedAt = DateTimeOffset.Now,
			UpdatedAt = DateTimeOffset.Now,
			UserId = user.Id,
		};

		customer = _context.Customers
								.Add(customer)
								.Entity;

		_context.SaveChanges();

		return Task.FromResult(ConvertCustomerToCustomerDto(customer));
	}

	public Task<bool> ArchiveCustomerAsync(Guid id)
	{
		var customer = _context.Customers.FirstOrDefault(c => c.Id == id);

		if (customer is null)
			return Task.FromResult(false);

		if (customer.Invoices.Count > 0)
			foreach (var item in customer.Invoices)
				_service.ArchiveInvoiceAsync(item.Id);

		customer.DeletedAt = DateTimeOffset.UtcNow;

		_context.Customers.Update(customer);
		_context.SaveChanges();

		return Task.FromResult(true);
	}

	public Task<bool> DeleteCustomerAsync(Guid CustomerId)
	{
		var customer = _context.Customers
								.Include(c => c.User)
								.Include(c => c.Invoices)
								.FirstOrDefault(c => c.Id == CustomerId);

		if (customer is null || customer.Invoices.Count > 0)
			return Task.FromResult(false);

		_context.Customers.Remove(customer);
		_context.SaveChanges();

		return Task.FromResult(true);
	}

	public Task<CustomerDto> GetCustomerByIdAsync(Guid id)
	{
		var customer = _context.Customers
								.Include(c => c.User)
								.FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(customer);

		if (customer.DeletedAt >= customer.CreatedAt)
			throw new KeyNotFoundException(nameof(customer));

		return Task.FromResult(ConvertCustomerToCustomerDto(customer));
	}

	public Task<PaginatedListDto<CustomerDto>> GetCustomersAsync(GetCustomersFilterDto filterDto)
	{
		var query = _context.Customers
							.Include(c => c.User)
							.Include(c => c.Invoices)
							.AsQueryable();

		query = query.Where(c => c.CreatedAt > c.DeletedAt);

		if (!string.IsNullOrEmpty(filterDto.SearchName))
			query = query.Where(i => i.Name.ToLower()!
									  .Contains(filterDto.SearchName.ToLower())!);

		if (!string.IsNullOrEmpty(filterDto.SearchEmail))
			query = query.Where(i => i.Email.ToLower()!
									  .Contains(filterDto.SearchEmail.ToLower()!));

		if (!string.IsNullOrEmpty(filterDto.SearchPhoneNumber))
			query = query.Where(i => i.PhoneNumber.ToLower()!
									  .Contains(filterDto.SearchPhoneNumber.ToLower()!));

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

		var dtoList = items.Select(ConvertCustomerToCustomerDto)
						   .ToList();

		return Task.FromResult(new PaginatedListDto<CustomerDto>
		{
			Items = dtoList,
			PageNumber = filterDto.PageNumber,
			PageSize = filterDto.PageSize,
			TotalCount = totalCount
		});
	}

	public Task<CustomerDto> EditCustomerAsync(Guid id, BaseCustomerDto customerDto)
	{
		var customer = _context.Customers
								.Include(c => c.User)
								.FirstOrDefault(i => i.Id == id);

		ArgumentNullException.ThrowIfNull(customer);

		if (!(string.IsNullOrEmpty(customerDto.Name)) && customer.Name != customerDto.Name)
			customer.Name = customerDto.Name;

		if (!(string.IsNullOrEmpty(customerDto.Email)) && customer.Email != customerDto.Email)
			customer.Email = customerDto.Email;

		if (!(string.IsNullOrEmpty(customerDto.PhoneNumber)) && customer.PhoneNumber != customerDto.PhoneNumber)
			customer.PhoneNumber = customerDto.PhoneNumber;

		_context.Customers.Update(customer);
		_context.SaveChanges();

		return Task.FromResult(ConvertCustomerToCustomerDto(customer));
	}


	#region Convert Methods

	public CustomerDto ConvertCustomerToCustomerDto(Customer customer)
	{
		return new()
		{
			Id = customer.Id,
			UserId = customer.UserId,
			Name = customer.Name,
			Email = customer.Email,
			PhoneNumber = customer.PhoneNumber,
			CreatedAt = customer.CreatedAt,
			UpdatedAt = customer.UpdatedAt,
		};
	}

	#endregion
}
