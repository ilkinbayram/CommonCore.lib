using System;

using CommonCore.Resources.Enums;




namespace CommonCore.Entities.Abstract
{
    public interface IUserBaseEntity
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        byte[] PasswordSalt { get; set; }
        byte[] PasswordHash { get; set; }
        DateTime? Birthday { get; set; }
        Gender Gender { get; set; }
        string SecurityToken { get; set; }
    }
}
