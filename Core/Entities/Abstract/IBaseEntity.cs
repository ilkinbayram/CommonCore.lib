using System;

using CommonCore.Resources.Enums;




namespace CommonCore.Entities.Abstract
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        string? Created_by { get; set; }
        DateTime? Created_at { get; set; }
    }
}
