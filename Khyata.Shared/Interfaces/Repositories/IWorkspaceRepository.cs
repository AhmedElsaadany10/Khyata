using khyata.Application.DTOs.Workspace;
using khyata.Domain.Entities;
using Khyata.Application.Common;

namespace khyata.Application.Interfaces.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<Result<WorkspaceDetailDto>> GetMyWorkspaceAsync(Guid workspaceId);
        //Task<Workspace?> GetByIdAsync(Guid workspaceId);
        Task UpdateAsync(Workspace workspace);
    }
}
