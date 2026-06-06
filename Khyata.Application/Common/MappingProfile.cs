using AutoMapper;
using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Application.DTOs.Admin.WorkspaceUser;
using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Customer.Requests;
using Khyata.Application.DTOs.Customer.Responses;
using Khyata.Application.DTOs.Employee;
using Khyata.Application.DTOs.Order;
using Khyata.Application.DTOs.Order.Payment;
using Khyata.Application.DTOs.Workspace;
using Khyata.Application.Helpers;
using Khyata.Domain.Entities;
using Khyata.Domain.Enums;
namespace Khyata.Application.Common
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        { // ── User / Auth ───────────────────────────────────────────────────────

            CreateMap<User, UserResponseDto>()
                    .ForMember(
                        dest => dest.WorkspaceName,
                        opt => opt.MapFrom(src => src.Workspace.Name)
                    )
                .ForMember(d => d.Role,
                    opt => opt.MapFrom(s => s.Role.ToString()));

            CreateMap<User, EmployeeResponseDto>()
                .ForMember(d => d.Role,
                    opt => opt.MapFrom(s => s.Role.ToString()));


            // ── Workspace ─────────────────────────────────────────────────────────

            CreateMap<Workspace, WorkspaceResponseDto>()
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<Workspace, WorkspaceDetailDto>()
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(s => s.Status.ToString()));

            // ── Customer ──────────────────────────────────────────────────────────

            CreateMap<CustomerPhone, CustomerPhoneDto>();

            CreateMap<Measurements, MeasurementsDto>();

            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(d => d.Phones,
                    opt => opt.MapFrom(c => c.Phones))

                .ForMember(d => d.Measurements,
                    opt => opt.MapFrom(c => c.Measurements));

            CreateMap<Customer, CustomersListItemDto>()
               .ForMember(d => d.PrimaryPhone,
                   opt => opt.MapFrom(c =>
                       c.Phones
                           .Where(p => p.IsPrimary)
                           .Select(p => p.Number)
                           .FirstOrDefault()));
            // ── Order ─────────────────────────────────────────────────────────────

            // ── Customer → OrderCustomerDto ─────────────────────────────
            CreateMap<Customer, OrderCustomerDto>()
                .ForMember(d => d.PrimaryPhone,
                    opt => opt.MapFrom(c =>
                        c.Phones
                         .Where(p => p.IsPrimary)
                         .Select(p => p.Number)
                         .FirstOrDefault()
                    ));

            // ── User → OrderCreatedByDto ────────────────────────────────
            CreateMap<User, OrderCreatedByDto>()
                .ForMember(d => d.Role,
                    opt => opt.MapFrom(s => s.Role.ToString()));

            // ── Order → OrderResponseDto ────────────────────────────────
            CreateMap<Order, OrderResponseDto>()
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(o => o.Status.ToString()))

                .ForMember(d => d.TotalPaid,
                    o => o.MapFrom(s => s.Payments.Sum(p => p.Amount)))

                .ForMember(d => d.RemainingAmount,
                    o => o.MapFrom(s => s.TotalPrice - s.Payments.Sum(p => p.Amount)))

                .ForMember(d => d.Payments,
                    o => o.MapFrom(s => s.Payments
                        .OrderBy(p => p.PaymentDate)))

               

                .ForMember(d => d.StatusHistory,
                    o => o.MapFrom(s => s.StatusHistory
                        .OrderBy(h => h.UpdatedAt)))

                .ForMember(d => d.Customer,
                    o => o.MapFrom(o => o.Customer))

                .ForMember(d => d.CreatedBy,
                    o => o.MapFrom(o => o.CreatedBy))

                .ForMember(d => d.AvailableStatuses,
                    o => o.MapFrom(o =>
                        OrderStatusRules.AllowedFrom(o.Status)
                            .Select(s => s.ToString())
                    ));

            // ── OrderStatusHistory → DTO ────────────────────────────────
            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
                .ForMember(d => d.FromStatus,
                    o => o.MapFrom(h => h.FromStatus.ToString()))

                .ForMember(d => d.ToStatus,
                    o => o.MapFrom(h => h.ToStatus.ToString()))

                .ForMember(d => d.UpdatedBy,
                    o => o.MapFrom(h =>
                        h.UpdatedBy != null
                            ? $"{h.UpdatedBy.Name}"
                            : "System"
                    ))

                .ForMember(d => d.UpdatedAt,
                    o => o.MapFrom(h => h.UpdatedAt));

            // ── OrderPayment → DTO ──────────────────────────────────────
            CreateMap<OrderPayment, OrderPaymentResponseDto>()
                .ForMember(d => d.ReceivedBy,
                    o => o.MapFrom(p =>
                        p.ReceivedBy != null
                            ? $"{p.ReceivedBy.Name}"
                            : "Unknown"
                    ));

           
            // Admin
            CreateMap<Workspace, WorkspaceSummaryDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.OwnerName, o => o.MapFrom(s =>
                    s.Users.FirstOrDefault(u => u.Role == Domain.Enums.WorkspaceRole.Owner) != null
                        ? s.Users.First(u => u.Role == Domain.Enums.WorkspaceRole.Owner).Name : "---"))
                .ForMember(d => d.OwnerPhone, o => o.MapFrom(s =>
                    s.Users.FirstOrDefault(u => u.Role == Domain.Enums.WorkspaceRole.Owner) != null
                        ? s.Users.First(u => u.Role == Domain.Enums.WorkspaceRole.Owner).Phone : "---"))
                .ForMember(d => d.TotalOrders, o => o.MapFrom(s => s.Orders.Count))
                .ForMember(d => d.TotalCustomers, o => o.MapFrom(s => s.Customers.Count))
                .ForMember(d => d.TotalEmployees, o => o.MapFrom(s =>
                    s.Users.Count(u => u.Role == Domain.Enums.WorkspaceRole.Employee && !u.IsDeleted)));
            CreateMap<AuditLog, AuditLogDto>();


        }
    }
}
