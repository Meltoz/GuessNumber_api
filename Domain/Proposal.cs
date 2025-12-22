using System.Text.RegularExpressions;

namespace Domain
{
    public class Proposal
    {
        public Guid Id { get; private set; } = Guid.Empty;

        public string Libelle { get; private set; }

        public string Response { get; private set; }

        public string? Source { get; private set; }

        public string? Author { get; private set; }

        private Proposal()
        {

        }

        public Proposal(string libelle, string response, string? source, string? author)
        {
            ChangeLibelle(libelle);
            ChangeResponse(response);
            if (!string.IsNullOrEmpty(author))
                ChangeAuthor(author);
            if (!string.IsNullOrEmpty(source))
                ChangeSource(source);
        }   
        
        public Proposal(Guid id, string libelle, string response, string? source, string? author) 
            : this(libelle, response, source, author)
        {
            if (id != Guid.Empty)
                Id = id;
        }

        public void ChangeLibelle(string newLibelle)
        {
            if (string.IsNullOrWhiteSpace(newLibelle))
                throw new ArgumentException(nameof(newLibelle));

            if (newLibelle.Length < 3)
                throw new ArgumentException("Libelle must be > 3 characters");

            if (newLibelle.Length > 200)
                throw new ArgumentException("Libelle must be <200 characters");

            var words = wordCounter(newLibelle);
            if (words < 3)
                throw new ArgumentException("Libelle must contains > 3 words");

            if (Libelle == newLibelle)
                return;

            Libelle = newLibelle;
        }

        public void ChangeResponse(string response)
        {
            string pattern = @"^\d+$";

            if (!Regex.IsMatch(response, pattern))
                throw new ArgumentException("Response is not an number");

            Response = response;
        }

        public void ChangeAuthor(string newAuthor)
        {
            if (newAuthor.Length < 3)
                throw new ArgumentException("Author must be >3 characters");

            if (newAuthor.Length > 50)
                throw new ArgumentException("Author must be <50 characters");

            Author = newAuthor;
        }

        public void ChangeSource(string newSource)
        {
            if (newSource.Length < 3)
                throw new ArgumentException("Source must be > 3 characters");

            if (!newSource.StartsWith("http"))
                throw new ArgumentException("Source is not a website");

            Source = newSource;
        }

        private int wordCounter(string text)
        {
            var patternWordCounter = @"\b\p{L}{2,}\b";

            var matchs = Regex.Matches(text, patternWordCounter);

            return matchs.Count;
        }

    }

}
