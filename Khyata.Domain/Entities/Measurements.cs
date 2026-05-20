namespace khyata.Domain.Entities
{
    public class Measurements
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid WorkspaceId { get; set; }
        public decimal? Height { get; set; }      // الطول
        public decimal? Sleeve { get; set; }     // الكم
        public decimal? ChestWidth { get; set; }       // عرض الصدر
        public decimal? Shoulder { get; set; }    // الكتف
        public decimal? Neck { get; set; }        // الرقبة
        public string? ExtraNotes { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Customer? Customer { get; set; }
    }
}