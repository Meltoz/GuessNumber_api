using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class CommunicationEntity : BaseEntity
    {
        [MaxLength(200)]
        public string Content { get; set; }
        
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }
    }
}
