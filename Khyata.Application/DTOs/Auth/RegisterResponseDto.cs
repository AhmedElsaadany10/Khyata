using Khyata.Application.DTOs.Workspace;

namespace Khyata.Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public string Message { get; set; } = default!;

        public UserResponseDto User { get; set; } = default!;

        public WorkspaceResponseDto Workspace { get; set; } = default!;
    }
}
