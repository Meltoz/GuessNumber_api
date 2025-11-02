using Domain.Enums;
using System.Text.RegularExpressions;

namespace Domain
{
    public class Report
    {
        public Guid Id { get; private set; } = Guid.Empty;

        public TypeReport Type { get; private set; }

        public ContextReport Context { get; private set; }

        public string Explanation { get; private set; } = string.Empty;

        public string? Mail { get; private set; }

        private Report()
        {
            
        }

        public Report(TypeReport type, ContextReport context, string explanation, string? mail)
        {
            ChangeConfiguration(type, context);
            ChangeExplanation(explanation);
            ChangeMail(mail);
        }

        public Report(Guid id, TypeReport type, ContextReport context, string explanation, string? mail) : this(type, context, explanation, mail)
        {
            if (id != Guid.Empty)
                Id = id;
        }

        public void ChangeExplanation(string explanation)
        {
            if (string.IsNullOrWhiteSpace(explanation))
                throw new ArgumentException("Explanation must be set.", "explanation");

            if (explanation.Length <= 5)
                throw new ArgumentException("Explanation must be length > 5");

            Explanation = explanation;
        }

        public void ChangeMail(string mail)
        {
            if (string.IsNullOrWhiteSpace(mail))
                return;

            var pattern = @"^(?!.*\s)(?!.*@@)(?!.*\.\.)(?!\.)[A-Za-z0-9._-]+(?<!\.)@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$";
            if (!Regex.IsMatch(mail, pattern))
                throw new ArgumentException("Mail is not valid.", "mail");

            Mail = mail;
        }

        public void ChangeConfiguration(TypeReport type, ContextReport context)
        {
            Type = type;
            Context = context;
        }
    }
}
