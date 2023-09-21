using CommonCore.Entities.Abstract;

namespace CommonCore.Core.Entities
{
    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public string? Created_by { get; set; }
        public string? Modified_by { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Modified_at { get; set; }
        public bool IsActive { get; set; }
    }
}
