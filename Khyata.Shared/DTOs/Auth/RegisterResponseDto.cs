using khyata.Application.DTOs.Workspace;

namespace khyata.Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public string Message { get; set; } = default!;

        public UserResponseDto User { get; set; } = default!;

        public WorkspaceResponseDto Workspace { get; set; } = default!;
    }
}
