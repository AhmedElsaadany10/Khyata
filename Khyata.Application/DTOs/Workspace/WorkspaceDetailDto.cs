namespace Khyata.Application.DTOs.Workspace
{
    public class WorkspaceDetailDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Status { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime? LastActivatedAt { get; set; }

        public DateTime? NextSuspensionDate { get; set; }
    }
}
