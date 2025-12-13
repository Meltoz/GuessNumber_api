namespace Infrastructure.Entities
{
    public class CategoryEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<QuestionEntity> Questions { get; set; }
    }
}
