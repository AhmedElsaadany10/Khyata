namespace khyata.Application.DTOs.Workspace
{
    public class WorkspaceResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Status { get; set; } = default!;
    }
}