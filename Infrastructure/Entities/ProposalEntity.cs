namespace Infrastructure.Entities
{
    public class ProposalEntity : BaseEntity
    {
        public string Libelle { get; set; } = string.Empty;

        public string Response { get; set; } = string.Empty;

        public string? Author { get; set; }

        public string? Source { get; set; }
    }
}
