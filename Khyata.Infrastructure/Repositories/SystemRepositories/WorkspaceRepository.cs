using AutoMapper;
using Khyata.Application.Common;
using Khyata.Application.DTOs.Workspace;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Entities;
using Khyata.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khyata.Infrastructure.Repositories.SystemRepositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WorkspaceRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //public Task<Workspace?> GetByIdAsync(Guid workspaceId)
        //{
            
        //}

        public async Task<Result<WorkspaceDetailDto>> GetMyWorkspaceAsync(Guid workspaceId)
        {
            var ws = await _context.Workspaces.FindAsync(workspaceId);
            return ws is null
                ? Result<WorkspaceDetailDto>.Failure(ApiError.NotFound("Workspace not found."))
                : Result<WorkspaceDetailDto>.Success(_mapper.Map<WorkspaceDetailDto>(ws));
        }

        public async Task<Result> UpdateNameAsync(Guid workspaceId, WorkspaceNameDto workspace)
        {
            var ws = await _context.Workspaces
        .FirstOrDefaultAsync(w => w.Id == workspaceId);

            if (ws is null)
                return Result.Failure(ApiError.NotFound("Workspace not found."));
          
            ws.Name = workspace.Name;
            _context.Workspaces.Update(ws);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
}
