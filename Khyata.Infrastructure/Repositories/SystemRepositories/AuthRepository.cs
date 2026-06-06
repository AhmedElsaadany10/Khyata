using AutoMapper;
using BCrypt.Net;
using Khyata.Application.Common;
using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Employee;
using Khyata.Application.DTOs.Workspace;
using Khyata.Application.Helpers;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Application.Interfaces.IServices;
using Khyata.Domain.Entities;
using Khyata.Domain.Enums;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Khyata.Infrastructure.Repositories.SystemRepositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthRepository(ITokenService tokenService, AppDbContext context, IMapper mapper)
        {
            _tokenService = tokenService;
            _context = context;
            _mapper = mapper;
        }

       
        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            if (!ValidationHelper.IsEgyptianPhone(dto.Phone))
            {
                return Result<AuthResponseDto>.Failure(
                    ApiError.BadRequest("Invalid phone number format."));
            }

            var user = await _context.Users
            .IgnoreQueryFilters()     // allow soft-deleted check so we give a meaningful error
            .Include(u => u.Workspace)
            .FirstOrDefaultAsync(u => u.Phone == dto.Phone);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Result<AuthResponseDto>.Failure(
                    ApiError.Unauthorized("Invalid phone number or password."));

            if (user.IsDeleted)
                return Result<AuthResponseDto>.Failure(
                    ApiError.Forbidden("This account has been deactivated."));

            return user.Workspace.Status switch
            {
                WorkspaceStatus.PendingActivation =>
                    Result<AuthResponseDto>.Failure(
                        ApiError.Forbidden("Your workspace is awaiting admin activation.")),

               WorkspaceStatus.Suspended =>
                    Result<AuthResponseDto>.Failure(
                        ApiError.Forbidden("Your workspace has been suspended. Please contact support.")),

               WorkspaceStatus.Active =>
                    Result<AuthResponseDto>.Success(new AuthResponseDto
                    {
                        AccessToken = _tokenService.GenerateToken(user),
                        TokenType = "Bearer",
                        ExpiresIn = _tokenService.ExpiresInSeconds,
                        User = _mapper.Map<UserResponseDto>(user),
                        Workspace = _mapper.Map<WorkspaceResponseDto>(user.Workspace)
                    }),

                _ =>
                    Result<AuthResponseDto>.Failure(
                        ApiError.Forbidden("Invalid workspace state."))
            };
        }

        public async Task<Result<RegisterResponseDto>> RegisterOwnerAsync(RegisterOwnerDto dto)
        {
            try
            {
                if (!ValidationHelper.IsEgyptianPhone(dto.Phone))
                {
                    return Result<RegisterResponseDto>.Failure(
                        ApiError.BadRequest("Please enter a valid Egyptian mobile number."));
                }

                if (!ValidationHelper.IsStrongPassword(dto.Password))
                {
                    return Result<RegisterResponseDto>.Failure(
                        ApiError.BadRequest("Password must contain uppercase, lowercase, number, and special character and be at least 8 characters."));
                }

                var phoneExists = await _context.Users
                    .IgnoreQueryFilters()
                    .AnyAsync(u => u.Phone == dto.Phone && u.Role == WorkspaceRole.Owner);

                if (phoneExists)
                    return Result<RegisterResponseDto>.Failure(
                        ApiError.Conflict("An owner account with this phone number already exists."));

                var workspace = new Workspace { Name = dto.WorkspaceName };
                var owner = new User
                {
                    WorkspaceId = workspace.Id,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = WorkspaceRole.Owner,
                   // CreatedBy = null // bootstrapped — no actor yet
                };
                workspace.Users.Add(owner);
                // Set audit FK after we have the ID
               // workspace.CreatedBy = owner.Id;

                _context.Workspaces.Add(workspace);
                await _context.SaveChangesAsync();

                return Result<RegisterResponseDto>.Success(new RegisterResponseDto
                    {
                        Message = "Registration submitted. Your account is pending admin activation.",
                        User = _mapper.Map<UserResponseDto>(owner),
                        Workspace = _mapper.Map<WorkspaceResponseDto>(workspace)
                    });
            }
            catch (DbUpdateException)
            {
                return Result<RegisterResponseDto>.Failure(ApiError.Internal("A database error occurred."));
            }
        }
    }
}
