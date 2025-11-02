namespace Web.ViewModels
{
    public class ReportVM
    {
        public Guid? Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Context { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public string? Mail { get; set; }
    }
}
