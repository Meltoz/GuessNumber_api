namespace Web.ViewModels.Admin
{
    public class ProposalAdminVM
    {
        public Guid Id { get; set; }

        public string Libelle { get; set; }

        public string Response { get; set; }

        public string? Author { get; set; }

        public string? Source { get; set; }
    }
}
