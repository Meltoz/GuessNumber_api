namespace Domain
{
    public class Communication
    {
        public Guid Id { get; private set; } = Guid.Empty;

        public DateTime? StartDate { get; private set; }

        public DateTime? EndDate { get; private set; }

        public string Content { get; private set; } = string.Empty;

        public Communication() { }

        public Communication(string content, DateTime? startDate, DateTime? endDate)
        {
            ChangeContent(content);
            ChangeDates(startDate, endDate);
        }

        public Communication(Guid id, string content, DateTime? startDate, DateTime? endDate) : this(content, startDate, endDate)
        {
            if (Id == Guid.Empty)
                Id = id;
        }

        public void ChangeContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must containt value");

            if (string.Equals(content, Content, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The new content must be different from the current content.");


            Content = content;
        }

        public void ChangeDates(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue && !endDate.HasValue)
                throw new ArgumentException("At least one of the two dates (start or end) must be defined.");

            if (startDate.HasValue && endDate.HasValue && startDate.Value >= endDate.Value)
                throw new ArgumentException("The start date must be strictly earlier than the end date.");


            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
