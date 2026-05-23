namespace Khyata.Application.DTOs.Customer.Requests
{
    public class MeasurementsDto
    {
        public decimal? Height { get; set; }

        public decimal? Sleeve { get; set; }

        public decimal? ChestWidth { get; set; }

        public decimal? Shoulder { get; set; }

        public decimal? Neck { get; set; }
        public string? Notes { get; set; }
    }
}