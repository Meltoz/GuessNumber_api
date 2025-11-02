namespace Domain
{
    public class Actuality
    {
        public Guid Id { get; private set; } = Guid.Empty;

        public string Title { get; private set; } = string.Empty;

        public string Content { get; private set; } = string.Empty;

        public DateTime StartPublish { get; private set; }

        public DateTime? EndPublish { get; private set; }

        private Actuality() { }

        public Actuality(string title, string content, DateTime? start, DateTime? end)
        {
            ChangeTitle(title);
            ChangeContent(content);
            ChangePublish(start, end);
        }

        public Actuality(Guid id, string title, string content, DateTime? start, DateTime? end)
        {
            if (Id == Guid.Empty)
                Id = id;

            ChangeTitle(title);
            ChangeContent(content);
            ChangePublish(start, end);
        }

        public void ChangeTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is not valid", "Title");

            if (title.Length > 100)
                throw new ArgumentException("Title must be max 100 length", "Title");

            if (title == Title)
                throw new ArgumentException($"Title is already define.", "Title");

            Title = title;
        }

        public void ChangePublish(DateTime? start, DateTime? end)
        {
            if (!start.HasValue && !end.HasValue)
                throw new ArgumentException("At least one of the two dates (start or end) must be defined.");

            if (start.HasValue && end.HasValue && start.Value >= end.Value)
                throw new ArgumentException("The start date must be strictly earlier than the end date.");

            if (!start.HasValue && end.HasValue)
            {
                EndPublish = end;
                return;
            }

            // Sinon on met à jour normalement
            StartPublish = start.Value;
            EndPublish = end;
        }

        public void ChangeContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must containt value");

            if (content == Content)
                throw new ArgumentException();

            Content = content;
        }
    }
}
