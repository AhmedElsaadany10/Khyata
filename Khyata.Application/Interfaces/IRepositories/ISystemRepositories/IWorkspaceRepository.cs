using Khyata.Application.DTOs.Workspace;
using Khyata.Domain.Entities;
using Khyata.Application.Common;

namespace Khyata.Application.Interfaces.IRepositories.ISystemRepositories
{
    public interface IWorkspaceRepository
    {
        Task<Result<WorkspaceDetailDto>> GetMyWorkspaceAsync(Guid workspaceId);
        Task<Result> UpdateNameAsync(Guid workspaceId, WorkspaceNameDto workspace);
    }
}
