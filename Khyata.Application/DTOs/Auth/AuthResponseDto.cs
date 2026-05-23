using Khyata.Application.DTOs.Workspace;

namespace Khyata.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string TokenType { get; set; } = default!;
        public int ExpiresIn { get; set; }

        public UserResponseDto User { get; set; } = default!;

        public WorkspaceResponseDto? Workspace { get; set; }
    }
}
