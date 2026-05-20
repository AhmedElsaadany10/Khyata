using AutoMapper;
using khyata.Application.DTOs.Auth;
using khyata.Application.DTOs.Customer.Requests;
using khyata.Application.DTOs.Customer.Responses;
using khyata.Application.DTOs.Employee;
using khyata.Application.DTOs.Order;
using khyata.Application.DTOs.Workspace;
using khyata.Domain.Enums;
using khyata.Application.Helpers;
using khyata.Domain.Entities;
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

            CreateMap<Customer, OrderCustomerDto>()
                .ForMember(d => d.PrimaryPhone,
                    opt => opt.MapFrom(c =>
                            c.Phones
                           .Where(p => p.IsPrimary)
                           .Select(p => p.Number)
                           .FirstOrDefault()));

            CreateMap<User, OrderCreatedByDto>()
                .ForMember(d => d.Role,
                    opt => opt.MapFrom(s => s.Role.ToString()));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(o => o.Status.ToString()))

                .ForMember(d => d.RemainingBalance,
                    opt => opt.MapFrom(o => o.RemainingBalance))

                .ForMember(d => d.Customer,
                    opt => opt.MapFrom(o => o.Customer))

                .ForMember(d => d.CreatedBy,
                    opt => opt.MapFrom(o => o.CreatedBy));
            // ... other mappings ...

            CreateMap<Order, OrderResponseDto>()
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(o => o.Status.ToString()))
                .ForMember(d => d.RemainingBalance,
                    opt => opt.MapFrom(o => o.RemainingBalance))
                .ForMember(d => d.Customer,
                    opt => opt.MapFrom(o => o.Customer))
                .ForMember(d => d.CreatedBy,
                    opt => opt.MapFrom(o => o.CreatedBy))
                .ForMember(d => d.AvailableStatuses,
                    opt => opt.MapFrom(o =>
                        OrderStatusRules.AllowedFrom(o.Status).Select(s => s.ToString())));




        }
    }
}
