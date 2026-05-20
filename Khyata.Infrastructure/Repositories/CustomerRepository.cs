using AutoMapper;
using khyata.Infrastructure.Persistence;
using khyata.Application.DTOs.Customer.Requests;
using khyata.Application.DTOs.Customer.Responses;
using khyata.Application.Helpers;
using khyata.Application.Interfaces.Repositories;
using khyata.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Khyata.Shared.Pagination;
using Khyata.Application.Common;

namespace khyata.Infrastructure.Repositories
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
            var phoneExists = await _context.CustomerPhones
           .AnyAsync(p => p.WorkspaceId == workspaceId && p.Number == dto.PrimaryPhone);
            if (phoneExists)
                return Result<CustomerResponseDto>.Failure(
                    ApiError.Conflict("This phone number is already registered in the workspace."));
            var customer = new Customer
            {
                WorkspaceId = workspaceId,
                Name = dto.Name,
                Address = dto.Address,
                CreatedBy = createdBy,
                Phones = [new CustomerPhone
            {
                WorkspaceId = workspaceId,
                Number      = dto.PrimaryPhone,
                IsPrimary   = true,
                CreatedBy   = createdBy
            }]
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
           .Include(c => c.Measurements)
           .Include(c => c.Phones)
           .FirstOrDefaultAsync(c => c.Id == customerId && c.WorkspaceId == workspaceId);

            return customer is null
                ? Result<CustomerResponseDto>.Failure(ApiError.NotFound("Customer not found."))
                : Result<CustomerResponseDto>.Success(_mapper.Map<CustomerResponseDto>(customer));
        }

        public async Task<Result<PagedResult<CustomersListItemDto>>> GetAllAsync(Guid workspaceId, CustomerQuery query)
        {
            var q = _context.Customers
           .Include(c => c.Phones)
           .Where(c => c.WorkspaceId == workspaceId);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                q = q.Where(c =>
                    c.Name.Contains(query.Search) ||
                    c.Phones.Any(p => p.Number.Contains(query.Search)));
            }

            q = q.OrderBy(c => c.Name);

            var result = await PaginationHelper.ToPagedResultAsync(
                q, query.Page, Math.Clamp(query.Limit, 1, 100),
                c => _mapper.Map<CustomersListItemDto>(c));

            return Result<PagedResult<CustomersListItemDto>>.Success(result);
        }
        public async Task<Result<CustomerPhoneDto>> AddPhoneAsync(Guid workspaceId, Guid customerId, AddCustomerPhoneDto dto)
        {
            var customer = await _context.Customers
           .Include(c => c.Phones)
           .FirstOrDefaultAsync(c => c.Id == customerId && c.WorkspaceId == workspaceId);

            if (customer is null)
                return Result<CustomerPhoneDto>.Failure(ApiError.NotFound("Customer not found."));
            var phoneExists = await _context.CustomerPhones
           .AnyAsync(p => p.WorkspaceId == workspaceId && p.Number == dto.Number);

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
                Number = dto.Number,
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
                customer.UpdatedBy = updatedBy;
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
    }
}
