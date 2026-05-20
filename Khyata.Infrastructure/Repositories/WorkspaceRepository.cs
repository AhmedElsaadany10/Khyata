using AutoMapper;
using khyata.Infrastructure.Persistence;
using khyata.Application.DTOs.Workspace;
using khyata.Application.Interfaces.Repositories;
using khyata.Domain.Entities;
using Khyata.Application.Common;

namespace khyata.Infrastructure.Repositories
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

        public async Task UpdateAsync(Workspace workspace)
        {
            _context.Workspaces.Update(workspace);
            await _context.SaveChangesAsync();
        }
    }
}
