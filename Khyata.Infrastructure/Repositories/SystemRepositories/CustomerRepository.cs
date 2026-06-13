using AutoMapper;
using Khyata.Application.Common;
using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Customer.Requests;
using Khyata.Application.DTOs.Customer.Responses;
using Khyata.Application.DTOs.Order;
using Khyata.Application.DTOs.Order.Payment;
using Khyata.Application.Helpers;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Entities;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Helpers;
using Khyata.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Khyata.Infrastructure.Repositories.SystemRepositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomerRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Result<CustomerResponseDto>> CreateAsync(Guid workspaceId, Guid createdBy, CreateCustomerDto dto)
        {
            if (!ValidationHelper.IsEgyptianPhone(dto.PrimaryPhone))
            {
                return Result<CustomerResponseDto>.Failure(
                    ApiError.BadRequest("Please enter a valid Egyptian mobile number."));
            }

            var normalizedPhone = PhoneHelper.NormalizeEgyptianPhone(dto.PrimaryPhone);
            var existingCustomer = await _context.Customers
                .IgnoreQueryFilters()
                .Include(c => c.Phones)
                .Include(c => c.Measurements)
                .FirstOrDefaultAsync(c =>
                    c.WorkspaceId == workspaceId &&
                    c.Phones.Any(p => p.Number == normalizedPhone));

            // Customer exists and is active
            if (existingCustomer is not null && !existingCustomer.IsDeleted)
            {
                return Result<CustomerResponseDto>.Failure(
                    ApiError.Conflict(
                        "This phone number is already registered in the workspace."));
            }

            // Customer exists but was soft-deleted => Restore
            if (existingCustomer is not null && existingCustomer.IsDeleted)
            {
                existingCustomer.IsDeleted = false;
                existingCustomer.DeletedAt = null;
                existingCustomer.UpdatedById = createdBy;
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                existingCustomer.Name = dto.Name;
                existingCustomer.Address = dto.Address;

                if (dto.Measurements is not null)
                {
                    if (existingCustomer.Measurements is null)
                    {
                        existingCustomer.Measurements = new Measurements
                        {
                            CustomerId = existingCustomer.Id,
                            WorkspaceId = workspaceId,
                            CreatedBy = createdBy,
                            Height = dto.Measurements.Height,
                            Sleeve = dto.Measurements.Sleeve,
                            ChestWidth = dto.Measurements.ChestWidth,
                            Shoulder = dto.Measurements.Shoulder,
                            Neck = dto.Measurements.Neck
                        };
                    }
                    else
                    {
                        existingCustomer.Measurements.Height = dto.Measurements.Height;
                        existingCustomer.Measurements.Sleeve = dto.Measurements.Sleeve;
                        existingCustomer.Measurements.ChestWidth = dto.Measurements.ChestWidth;
                        existingCustomer.Measurements.Shoulder = dto.Measurements.Shoulder;
                        existingCustomer.Measurements.Neck = dto.Measurements.Neck;
                        existingCustomer.Measurements.UpdatedBy = createdBy;
                    }
                }

                await _context.SaveChangesAsync();

                return await GetByIdAsync(workspaceId, existingCustomer.Id);
            }

            // Create new customer
            var customer = new Customer
            {
                WorkspaceId = workspaceId,
                Name = dto.Name,
                Address = dto.Address,
                CreatedById = createdBy,
                Phones =
                [
                    new CustomerPhone
            {
                WorkspaceId = workspaceId,
                Number = normalizedPhone,
                IsPrimary = true,
                CreatedBy = createdBy
            }
                ]
            };

            if (dto.Measurements is not null)
            {
                customer.Measurements = new Measurements
                {
                    CustomerId = customer.Id,
                    WorkspaceId = workspaceId,
                    Height = dto.Measurements.Height,
                    Sleeve = dto.Measurements.Sleeve,
                    ChestWidth = dto.Measurements.ChestWidth,
                    Shoulder = dto.Measurements.Shoulder,
                    Neck = dto.Measurements.Neck,
                    CreatedBy = createdBy
                };
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(workspaceId, customer.Id);
        }
        public async Task<Result<CustomerResponseDto>> GetByIdAsync(Guid workspaceId, Guid customerId)
        {
            var customer = await _context.Customers
                .Where(c => c.Id == customerId && c.WorkspaceId == workspaceId)
                .Select(c => new CustomerResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy != null ? c.CreatedBy.Name : null,

                    OrdersCount = c.Orders.Count,

                    Measurements = c.Measurements == null ? null : new MeasurementsDto
                    {
                        Height = c.Measurements.Height,
                        Sleeve = c.Measurements.Sleeve,
                        ChestWidth = c.Measurements.ChestWidth,
                        Shoulder = c.Measurements.Shoulder,
                        Neck = c.Measurements.Neck
                    },

                    Phones = c.Phones
                        .Select(p => new CustomerPhoneDto
                        {
                            Id = p.Id,
                            Number = p.Number,
                            IsPrimary = p.IsPrimary
                        }).ToList(),

                    Orders = c.Orders
                        .Select(o => new CustomerOrderSummaryDto
                        {
                            Id = o.Id,
                            Description = o.Description,
                            TotalPrice = o.TotalPrice,
                            TotalPaid = o.Payments.Sum(p => p.Amount), 
                            Status = o.Status.ToString(),
                            CreatedAt = o.CreatedAt,
                            CreatedBy = o.CreatedBy != null ? o.CreatedBy.Name : null
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return customer is null
                ? Result<CustomerResponseDto>.Failure(ApiError.NotFound("Customer not found."))
                : Result<CustomerResponseDto>.Success(customer);
        }

        public async Task<Result<PagedResult<CustomersListItemDto>>> GetAllAsync(Guid workspaceId, CustomerQuery query)
        {
            var q = _context.Customers
             .Where(c => c.WorkspaceId == workspaceId)
             .Select(c => new CustomersListItemDto
             {
                 Id = c.Id,
                 Name = c.Name,
                 Address = c.Address,

                 PrimaryPhone = c.Phones
                     .Where(p => p.IsPrimary)
                     .Select(p => p.Number)
                     .FirstOrDefault() ?? "",

                 OrdersCount = c.Orders.Count()
             });

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                q = q.Where(c =>
                    c.Name.Contains(query.Search) ||
                    c.PrimaryPhone.Contains(query.Search));
            }

            q = q.OrderBy(c => c.Name);

            var result = await PaginationHelper.ToPagedResultAsync(
                q, query.Page, Math.Clamp(query.Limit, 1, 100),
                c => c );

            return Result<PagedResult<CustomersListItemDto>>.Success(result);
        }
        public async Task<Result<CustomerPhoneDto>> AddPhoneAsync(Guid workspaceId, Guid customerId, AddCustomerPhoneDto dto)
        {
            var normalizedPhone = PhoneHelper.NormalizeEgyptianPhone(dto.Number);

            if (!ValidationHelper.IsEgyptianPhone(normalizedPhone))
            {
                return Result<CustomerPhoneDto>.Failure(
                    ApiError.BadRequest("Please enter a valid Egyptian mobile number."));
            }

            var customer = await _context.Customers
           .Include(c => c.Phones)
           .FirstOrDefaultAsync(c => c.Id == customerId && c.WorkspaceId == workspaceId);

            if (customer is null)
                return Result<CustomerPhoneDto>.Failure(ApiError.NotFound("Customer not found."));
            var phoneExists = await _context.CustomerPhones
           .AnyAsync(p => p.WorkspaceId == workspaceId && p.Number == normalizedPhone);

            if (phoneExists)
                return Result<CustomerPhoneDto>.Failure(
                    ApiError.Conflict("This phone number is already registered in the workspace."));
            // If this new phone is set as primary, demote the current primary
            if (dto.IsPrimary)
            {
                foreach (var existing in customer.Phones.Where(p => p.IsPrimary))
                    existing.IsPrimary = false;
            }
            var phone = new CustomerPhone
            {
                CustomerId = customerId,
                WorkspaceId = workspaceId,
                Number = normalizedPhone,
                IsPrimary = dto.IsPrimary
            };

            _context.CustomerPhones.Add(phone);
            await _context.SaveChangesAsync();

            return Result<CustomerPhoneDto>.Success(_mapper.Map<CustomerPhoneDto>(phone));
        }
        public async Task<Result> RemovePhoneAsync(Guid workspaceId, Guid customerId, Guid phoneId)
        {
            var phone = await _context.CustomerPhones
            .FirstOrDefaultAsync(p =>
                p.Id == phoneId &&
                p.CustomerId == customerId &&
                p.WorkspaceId == workspaceId);
            if (phone is null)
                return Result.Failure(ApiError.NotFound("Phone number not found."));
            // Don't allow removal of the last phone
            var count = await _context.CustomerPhones
                .CountAsync(p => p.CustomerId == customerId);
            if (count <= 1)
                return Result.Failure(
                    ApiError.BadRequest("Cannot remove the last phone number. Add another first."));
            // If removing the primary, promote the next available
            if (phone.IsPrimary)
            {
                var next = await _context.CustomerPhones
                    .Where(p => p.CustomerId == customerId && p.Id != phoneId)
                    .FirstOrDefaultAsync();
                if (next is not null) next.IsPrimary = true;
            }
            _context.CustomerPhones.Remove(phone);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<CustomerResponseDto>> UpdateAsync(Guid workspaceId, Guid customerId, Guid updatedBy, UpdateCustomerDto dto)
        {
            var customer = await _context.Customers
           .Include(c => c.Measurements)
           .Include(c => c.Phones)
           .FirstOrDefaultAsync(c => c.Id == customerId && c.WorkspaceId == workspaceId);
            if (customer is null)
                return Result<CustomerResponseDto>.Failure(ApiError.NotFound("Customer not found."));
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                customer.Name = dto.Name;
                customer.Address = dto.Address;
                customer.UpdatedById = updatedBy;
            }
            if (dto.Measurements is not null)
            {
                if (customer.Measurements is null)
                {
                    customer.Measurements = new Measurements
                    {
                        CustomerId = customer.Id,
                        WorkspaceId = workspaceId,
                        CreatedBy = updatedBy
                    };
                }
                var m = customer.Measurements;
                m.Height = dto.Measurements.Height ?? m.Height;
                m.Sleeve = dto.Measurements.Sleeve ?? m.Sleeve;
                m.ChestWidth = dto.Measurements.ChestWidth ?? m.ChestWidth;
                m.Shoulder = dto.Measurements.Shoulder ?? m.Shoulder;
                m.Neck = dto.Measurements.Neck ?? m.Neck;
                m.UpdatedBy = updatedBy;
            }
            await _context.SaveChangesAsync();
            return Result<CustomerResponseDto>.Success(_mapper.Map<CustomerResponseDto>(customer));
        }
        public async Task<Result> RemoveAsync( Guid workspaceId,Guid customerId, Guid deletedBy)
        {
            var customer = await _context.Customers
                .Include(c => c.Phones)
                .FirstOrDefaultAsync(c =>
                    c.Id == customerId &&
                    c.WorkspaceId == workspaceId);

            if (customer is null)
                return Result.Failure(
                    ApiError.NotFound("Customer not found."));

            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;
            customer.UpdatedById = deletedBy;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
